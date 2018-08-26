package jp.ne.pickle.libpixelreads;

import android.content.Context;
import android.media.AudioFormat;
import android.media.MediaCodec;
import android.media.MediaCodecInfo;
import android.media.MediaCodecList;
import android.media.MediaFormat;
import android.media.MediaMuxer;
import android.os.Environment;
import android.os.Handler;
import android.provider.ContactsContract;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.ByteBuffer;


public class UnityConnect
{
    public static final String VCODEC = "video/avc";
    public static int vBitrate = 1200 * 1024;  // 1200 kbps
    public static final int VFPS = 30;
    public static final int VGOP = 30;
    public static final String ACODEC = "audio/mp4a-latm";
    public static final int aBitrate = 64 * 1024;
    public static final int aRagTime = 0 * 1000;    // RagTime 200ms

    private static UnityConnect m_Instance;

    private TextureCapture textureCapture;
    private MediaCodec vencoder;
    private MediaCodecInfo vmci;
    private MediaCodec.BufferInfo vebi = new MediaCodec.BufferInfo();
    private MediaCodec.BufferInfo aebi = new MediaCodec.BufferInfo();
    private MediaCodec aencoder;
    private MediaMuxer mMuxer;
    private boolean mMuxerStart;
    private boolean mVideoReady;
    private boolean mAudioReady;
    private boolean mMuxerStarted;
    private boolean mEndOfStream;
    private Handler mStopHandler;
    private int mResolutionWidth;
    private int mResolutionHeight;
    private int mVideoEncodeType;
    private int mVideoTrackIndex;
    private int mAudioTrackIndex;
    private long mPresentTimeUs;
    private int mAudioChannnel;
    private int mVideoType;
    private long mAudioPts;
    private int mSampleRate;


    static public void destroyInstance()
    {
        if (m_Instance != null)
        {
            UnityPlayer.currentActivity.runOnUiThread(new Runnable()
            {
                @Override
                public void run()
                {
                    m_Instance.releaseEncoder();
                    if (m_Instance.mMuxer != null) {
                        // TODO: stop() throws an exception if you haven't fed it any data.  Keep track
                        //       of frames submitted, and don't call stop() if we haven't written anything.
                        m_Instance.mMuxer.stop();
                        m_Instance.mMuxer.release();
                        m_Instance.mMuxer = null;
                    }
                    m_Instance = null;
                }
            });
        }
    }

    static public void initialize(int resolutionW, int resolutionH, int audioChannel, int sampleRate)
    {
        if (m_Instance == null)
        {
            m_Instance = new UnityConnect();
        }
        m_Instance.initializeProc(resolutionW, resolutionH, audioChannel, sampleRate);
    }

    static public boolean record(String filePath)
    {
        if (m_Instance == null)
        {
            return false;
        }

        return m_Instance.recordProc(filePath);
    }

    static public void stop()
    {
        if (m_Instance != null)
        {
            m_Instance.stopRequestProc();
        }
    }

    static public void update()
    {
        if (m_Instance != null)
        {
            m_Instance.updateProc();
        }
    }

    static public void sendRgbaFrame(int dataPtr)
    {
        if (m_Instance != null)
        {
            m_Instance.sendRgbaFrameProc(dataPtr);
        }
    }

    static public void sendWaveData(byte[] data)
    {
        if (m_Instance != null)
        {
            m_Instance.sendWaveDataProc(data);
        }
    }

    static public void transByteData(byte[] data)
    {
        Log.d("", "transByteData:" + data.length);
    }


    private void initializeProc(int resolutionW, int resolutionH, int audioChannel, int sampleRate)
    {
        textureCapture = new TextureCapture();
        textureCapture.usePBO();
        textureCapture.flipY();
        textureCapture.loadSamplerShaderProg(UnityPlayer.currentActivity, R.raw.vertex, R.raw.fragment);
        textureCapture.init();
        textureCapture.onInputSizeChanged(resolutionW, resolutionH);

        mVideoEncodeType = chooseVideoEncoder();
        Log.i("UnityConnect", String.format("chooseVideoEncoder %d", mVideoEncodeType));

        mResolutionWidth = resolutionW;
        mResolutionHeight = resolutionH;
        mAudioChannnel = audioChannel;
        mSampleRate = sampleRate;
        mVideoTrackIndex = -1;
        mAudioTrackIndex = -1;
        mMuxerStart = false;
        mMuxerStarted = false;
        mVideoReady = false;
        mAudioReady = mAudioChannnel == 0;
        mEndOfStream = false;
    }

    private void createAndStartEncoder()
    {
        MediaFormat videoFormat = MediaFormat.createVideoFormat(VCODEC, mResolutionWidth, mResolutionHeight);
        videoFormat.setInteger(MediaFormat.KEY_COLOR_FORMAT, mVideoEncodeType);
        videoFormat.setInteger(MediaFormat.KEY_MAX_INPUT_SIZE, 0);
        videoFormat.setInteger(MediaFormat.KEY_BIT_RATE, vBitrate);
        videoFormat.setInteger(MediaFormat.KEY_FRAME_RATE, VFPS);
        videoFormat.setInteger(MediaFormat.KEY_I_FRAME_INTERVAL, VGOP / VFPS);
        try {
            vencoder = MediaCodec.createEncoderByType(VCODEC);
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }
        vencoder.configure(videoFormat, null, null, MediaCodec.CONFIGURE_FLAG_ENCODE);
        mVideoType = mVideoEncodeType == MediaCodecInfo.CodecCapabilities.COLOR_FormatYUV420SemiPlanar ? 2 : 1;
        vencoder.start();

        if (mAudioChannnel != 0) {
            MediaFormat audioFormat = MediaFormat.createAudioFormat(ACODEC, mSampleRate, mAudioChannnel);
            audioFormat.setInteger(MediaFormat.KEY_AAC_PROFILE, MediaCodecInfo.CodecProfileLevel.AACObjectLC);
            audioFormat.setInteger(MediaFormat.KEY_CHANNEL_MASK, mAudioChannnel == 1 ? AudioFormat.CHANNEL_IN_MONO : AudioFormat.CHANNEL_IN_STEREO);
            audioFormat.setInteger(MediaFormat.KEY_BIT_RATE, aBitrate);
            try {
                aencoder = MediaCodec.createEncoderByType(ACODEC);
            } catch (IOException e) {
                e.printStackTrace();
                return;
            }
            aencoder.configure(audioFormat, null, null, MediaCodec.CONFIGURE_FLAG_ENCODE);
            aencoder.start();
        }
    }

    private void releaseEncoder()
    {
        if (m_Instance.vencoder != null) {
            m_Instance.vencoder.flush();
            m_Instance.vencoder.stop();
            m_Instance.vencoder.release();
            m_Instance.vencoder = null;
        }
        if (m_Instance.aencoder != null) {
            m_Instance.aencoder.flush();
            m_Instance.aencoder.stop();
            m_Instance.aencoder.release();
            m_Instance.aencoder = null;
        }
        mVideoTrackIndex = -1;
        mAudioTrackIndex = -1;
    }

    private boolean recordProc(String filePath)
    {
        if (mMuxerStart) {
            return false;
        }
        try {
            File file = new File(filePath);
            File dir = new File(file.getParent());
            if (!dir.exists()) {
                dir.mkdirs();
            }
            else if (file.exists()) {
                file.delete();
            }
            createAndStartEncoder();
            mMuxer = new MediaMuxer(filePath, MediaMuxer.OutputFormat.MUXER_OUTPUT_MPEG_4);
            mMuxerStart = true;
            mPresentTimeUs = System.nanoTime() / 1000;
            mAudioPts = 0;
            mEndOfStream = false;
            return true;
        } catch (IOException e) {
            e.printStackTrace();
            return false;
        }
    }

    private void stopRequestProc()
    {
        if (mMuxerStart && !mEndOfStream) {
            mStopHandler = new Handler();
            final Runnable r = new Runnable() {
                @Override
                public void run() {
                    // UIスレッド
                    if (mStopHandler != null)
                    {
                        onProcessedYuvFrame(null, (System.nanoTime() / 1000) - mPresentTimeUs);
                        mStopHandler.postDelayed(this, 100);
                    }
                }
            };
            mStopHandler.post(r);
        }
    }

    private void stopProc()
    {
        if (mMuxerStart) {
            if (mStopHandler != null)
            {
                mStopHandler = null;
            }
            try {
                mMuxer.stop();
                mMuxer.release();
            } catch(Exception e){}
            releaseEncoder();
            mMuxer = null;
            mMuxerStart = false;
            mMuxerStarted = false;
            mVideoReady = false;
            mAudioReady = mAudioChannnel == 0;
            mEndOfStream = false;
        }
    }

    private void sendRgbaFrameProc(int dataPtr)
    {
        textureCapture.onDrawFrame(dataPtr, false);
    }

    private void updateProc()
    {
        int dataType = mMuxerStart? mVideoType : 0;
        byte[] data = textureCapture.getGLFboBuffer(dataType);
        if (mMuxerStart && data != null) {
            onProcessedYuvFrame(data, (System.nanoTime() / 1000) - mPresentTimeUs);

/*
            File pathExternalPublicDir = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS);
            String filePath = pathExternalPublicDir.getPath() + "/sample.dat";
            File file = new File(filePath);
            file.getParentFile().mkdir();
            FileOutputStream fos;
            try {
                fos = new FileOutputStream(file, false);
                fos.write(data);
                fos.flush();
                fos.close();
            } catch (Exception e) {
            }
*/
        }
    }

    private void onProcessedYuvFrame(byte[] yuvFrame, long pts) {
        if (vencoder == null) {
            return;
        }

        ByteBuffer[] inBuffers = vencoder.getInputBuffers();
        int inBufferIndex = vencoder.dequeueInputBuffer(-1);
        if (inBufferIndex >= 0) {
            ByteBuffer bb = inBuffers[inBufferIndex];
            bb.clear();
            if (yuvFrame != null && !mEndOfStream) {
                bb.put(yuvFrame, 0, yuvFrame.length);
                Log.d("UnityConnect", "pts(Video)" + pts);
                vencoder.queueInputBuffer(inBufferIndex, 0, yuvFrame.length, pts, MediaCodec.BUFFER_FLAG_CODEC_CONFIG);
            } else {
                vencoder.queueInputBuffer(inBufferIndex, 0, 0, pts, MediaCodec.BUFFER_FLAG_END_OF_STREAM);
                mEndOfStream = true;
                Log.d("UnityConnect", "BUFFER_FLAG_END_OF_STREAM");
            }
        }

        ByteBuffer[] outBuffers = vencoder.getOutputBuffers();
        while (true) {
            int encoderStatus = vencoder.dequeueOutputBuffer(vebi, 0);
            Log.i("UnityConnect", String.format("dequeueOutputBuffer %d", encoderStatus));
            if (encoderStatus == MediaCodec.INFO_OUTPUT_BUFFERS_CHANGED) {
                outBuffers = vencoder.getOutputBuffers();
            } else if (encoderStatus == MediaCodec.INFO_OUTPUT_FORMAT_CHANGED) {
                if (mMuxerStarted) {
                    throw new RuntimeException("format changed twice");
                }
                mVideoTrackIndex = mMuxer.addTrack(vencoder.getOutputFormat());
                mVideoReady = true;
                if (mVideoReady && (mAudioReady || mAudioChannnel == 0)) {
                    mMuxer.start();
                    mMuxerStarted = true;
                    //Log.d("UnityConnect", "Start Muxer(Video)");
                }
            } else if (encoderStatus < 0) {
                break;
            } else  {
                if (mMuxerStarted && mVideoTrackIndex >= 0) {
                    ByteBuffer encodedData = outBuffers[encoderStatus];
                    if (encodedData == null) {
                        throw new RuntimeException("encoderOutputBuffer " + encoderStatus + " was null");
                    }

                    //Log.d("UnityConnect", "vebi.flags" + vebi.flags);
                    if ((vebi.flags & MediaCodec.BUFFER_FLAG_CODEC_CONFIG) != 0) {
                        // The codec config data was pulled out and fed to the muxer when we got
                        // the INFO_OUTPUT_FORMAT_CHANGED status.  Ignore it.
                        //vebi.size = 0;
                    }

                    //Log.d("UnityConnect", "vebi.size" + vebi.size);
                    if (vebi.size != 0) {
                        // adjust the ByteBuffer values to match BufferInfo (not needed?)
                        encodedData.position(vebi.offset);
                        encodedData.limit(vebi.offset + vebi.size);

                        //Log.d("UnityConnect", "Write Video");
                        mMuxer.writeSampleData(mVideoTrackIndex, encodedData, vebi);
                    }
                    vencoder.releaseOutputBuffer(encoderStatus, false);

                    if ((vebi.flags & MediaCodec.BUFFER_FLAG_END_OF_STREAM) != 0) {
                        Log.d("UnityConnect", "stopProc");
                        stopProc();
                        break;      // out of while
                    }
                }
                else
                {
                    vencoder.releaseOutputBuffer(encoderStatus, false);
                }
            }
        }
    }

    private void sendWaveDataProc(byte[] data)
    {
        if (!mMuxerStart || mAudioChannnel == 0) {
            return;
        }

        //long pts = (System.nanoTime() / 1000) - mPresentTimeUs + aRagTime;
        ByteBuffer[] inBuffers = aencoder.getInputBuffers();
        ByteBuffer[] outBuffers = aencoder.getOutputBuffers();
        int remain = data.length;
        int ofs = 0;
        while (remain  != 0) {
            int inBufferIndex = aencoder.dequeueInputBuffer(-1);
            if (inBufferIndex >= 0) {
                ByteBuffer bb = inBuffers[inBufferIndex];
                bb.clear();
                int size = (bb.capacity() < remain) ? bb.capacity() : remain;
                bb.put(data, ofs, size);
                remain -= size;
                ofs += size;
                //Log.d("UnityConnect", "Audio pts " + mAudioPts + " size " + size + " Remain " + remain + " " + inBufferIndex);
                aencoder.queueInputBuffer(inBufferIndex, 0, size, mAudioPts + aRagTime, 0);
                mAudioPts += (1000000L * size) / (mSampleRate * mAudioChannnel * 2);
            }

            while (true) {
                int outBufferIndex = aencoder.dequeueOutputBuffer(aebi, 0);
                //Log.d("UnityConnect", "dequeueOutputBuffer(Audio)" + outBufferIndex);
                if (outBufferIndex == MediaCodec.INFO_OUTPUT_FORMAT_CHANGED) {
                    if (mAudioTrackIndex >= 0) {
                        throw new RuntimeException("format changed twice");
                    }
                    mAudioTrackIndex = mMuxer.addTrack(aencoder.getOutputFormat());
                    mAudioReady = true;
                    if (mVideoReady && mAudioReady) {
                        mMuxer.start();
                        mMuxerStarted = true;
                        //Log.d("UnityConnect", "Start Muxer(Audio)");
                    }
                }
                else if (outBufferIndex == MediaCodec.INFO_OUTPUT_BUFFERS_CHANGED) {
                    outBuffers = aencoder.getOutputBuffers();
                } else if (outBufferIndex < 0) {
                    break;
                } else {
                    if (mMuxerStarted && mAudioTrackIndex >= 0) {
                        //Log.d("UnityConnect", "Write Audio");
                        ByteBuffer encodedData = outBuffers[outBufferIndex];
                        mMuxer.writeSampleData(mAudioTrackIndex, encodedData, aebi);
                    }
                    aencoder.releaseOutputBuffer(outBufferIndex, false);
                }
            }
        }
    }

    // choose the video encoder by name.
    private MediaCodecInfo chooseVideoEncoder(String name) {
        int nbCodecs = MediaCodecList.getCodecCount();
        for (int i = 0; i < nbCodecs; i++) {
            MediaCodecInfo mci = MediaCodecList.getCodecInfoAt(i);
            if (!mci.isEncoder()) {
                continue;
            }

            String[] types = mci.getSupportedTypes();
            for (int j = 0; j < types.length; j++) {
                if (types[j].equalsIgnoreCase(VCODEC)) {
                    Log.i("UnityConnect", String.format("vencoder %s types: %s", mci.getName(), types[j]));
                    if (name == null) {
                        return mci;
                    }

                    if (mci.getName().contains(name)) {
                        return mci;
                    }
                }
            }
        }

        return null;
    }

    private int chooseVideoEncoder() {
        vmci = chooseVideoEncoder(null);
        int matchedColorFormat = 0;
        MediaCodecInfo.CodecCapabilities cc = vmci.getCapabilitiesForType(VCODEC);
        for (int i = 0; i < cc.colorFormats.length; i++) {
            int cf = cc.colorFormats[i];
            Log.i("UnityConnect", String.format("vencoder %s supports color format 0x%x(%d)", vmci.getName(), cf, cf));
            if (cf >= cc.COLOR_FormatYUV420Planar && cf <= cc.COLOR_FormatYUV420SemiPlanar) {
                if (cf > matchedColorFormat) {
                    matchedColorFormat = cf;
                }
            }
        }
        return matchedColorFormat;
    }
}
