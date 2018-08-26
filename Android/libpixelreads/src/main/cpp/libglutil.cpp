#include <jni.h>
#include <GLES3/gl3.h>
#include "libyuv.h"

#include <android/log.h>

#define LIBENC_LOGD(...) ((void)__android_log_print(ANDROID_LOG_DEBUG, "libglutil", __VA_ARGS__))
#define LIBENC_LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO , "libglutil", __VA_ARGS__))
#define LIBENC_LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN , "libglutil", __VA_ARGS__))
#define LIBENC_LOGE(...) ((void)__android_log_print(ANDROID_LOG_ERROR, "libglutil", __VA_ARGS__))

#define LIBENC_ARRAY_ELEMS(a)  (sizeof(a) / sizeof(a[0]))

static JavaVM *gjvm= NULL;
static jobject gClassLoader = NULL;
static jmethodID gFindClassMethod = NULL;

static JNIEnv *gjenv= NULL;
static jclass gjClass_TextureCapture = NULL;
static jmethodID gjMethodId_callNativeMethod = NULL;

static jclass gjClass_UnityConnect   = NULL;
static jmethodID gjMethodId_sendRgbaFrame = NULL;

JNIEnv* getEnv();
jclass findClass(JNIEnv *env, const char* name);

extern "C" {
    using UnityRenderEvent = void(*)(int);

    UnityRenderEvent getRenderEventFunc();
    void libglutil_SetPixels(int ptr, int width, int height);
}

// call from Unity(IssuePluginEvent)
void onRenderEvent(int eventId)
{
    //LIBENC_LOGD("onRenderEvent:%d", eventId);
    JNIEnv *jenv = getEnv();
    if (gjClass_UnityConnect == NULL)
    {
        gjClass_UnityConnect = (jclass)jenv->NewGlobalRef(findClass(jenv, "jp/ne/pickle/libpixelreads/UnityConnect"));
        gjMethodId_sendRgbaFrame = jenv->GetStaticMethodID(gjClass_UnityConnect, "sendRgbaFrame", "(I)V");
    }
    if (gjClass_UnityConnect != NULL)
    {
        jenv->CallStaticVoidMethod(gjClass_UnityConnect, gjMethodId_sendRgbaFrame, eventId);
        jthrowable mException = jenv->ExceptionOccurred();
        if (mException)
        {
            jenv->ExceptionDescribe();
            jenv->ExceptionClear();
        }
    }
    else
    {
        LIBENC_LOGE("jClass not got");
    }
}

// call from Unity(IssuePluginEvent)
UnityRenderEvent getRenderEventFunc()
{
    return onRenderEvent;
}

// call from C#
void libglutil_SetPixels(int ptr, int width, int height)
{
    LIBENC_LOGD("SetPixels:%d, %d", width, height);

    int size = width * height * 4;
    jbyteArray buffer = gjenv->NewByteArray(size);
    gjenv->SetByteArrayRegion(buffer, 0, size, (jbyte *)ptr);

    gjenv->CallStaticVoidMethod(gjClass_TextureCapture, gjMethodId_callNativeMethod, buffer, width, height);
}

// call from Java
static void libglutil_ReadPixels(JNIEnv *env, jobject thiz, jint x, jint y, jint width, jint height, jint pboId)
{
    glBindBuffer(GL_PIXEL_PACK_BUFFER, pboId);
    glReadBuffer(GL_COLOR_ATTACHMENT0);
    glReadPixels(x, y, width, height, GL_RGBA, GL_UNSIGNED_BYTE, 0);
    //LIBENC_LOGD("glReadPixels:%d", glGetError());
    glBindBuffer(GL_PIXEL_PACK_BUFFER, 0);
}

static jbyteArray libglutil_ConvertDataProc(JNIEnv *env, jobject thiz, jbyte *srcBuff, jint width, jint height, jint type)
{
    int srcSize = width * height;
    uint8* dstBuff = NULL;
    int dstSize;
    if (type == 0)
    {
        dstSize = srcSize << 2;
    }
    else
    {
        dstSize = srcSize + (srcSize >> 1);
        if (srcBuff != NULL)
        {
            dstBuff = new uint8[dstSize];
            //libyuv::ABGRToARGB((const uint8*)srcBuff, width << 2, (uint8*)srcBuff, width << 2, width, height);
            if (type == 1)
            {
                libyuv::ARGBToI420((const uint8*)srcBuff, width << 2, dstBuff, width, dstBuff + srcSize, width >> 1, dstBuff + srcSize + (srcSize >> 2), width >> 1, width, height);
            }
            else
            {
                libyuv::ARGBToNV21((const uint8*)srcBuff, width << 2, dstBuff, width, dstBuff + srcSize, width, width, height);
            }
        }
    }
    jbyteArray buffer = env->NewByteArray(dstSize);
    if (srcBuff != NULL) {
        env->SetByteArrayRegion(buffer, 0, dstSize, (dstBuff != NULL? (jbyte *)dstBuff : srcBuff));
    }
    delete[] dstBuff;

    return buffer;
}

static jbyteArray libglutil_ConvertData(JNIEnv *env, jobject thiz, jbyteArray data, jint width, jint height, jint type)
{
    jbyte *srcBuff = env->GetByteArrayElements(data, NULL);
    return libglutil_ConvertDataProc(env, thiz, srcBuff, width, height, type);
}

// call from Java
static jbyteArray libglutil_GetPboBuffer(JNIEnv *env, jobject thiz, jint width, jint height, jint pboId, jint type)
{
    glBindBuffer(GL_PIXEL_PACK_BUFFER, pboId);
    void* srcBuff = glMapBufferRange(GL_PIXEL_PACK_BUFFER, 0, (width * height) << 2, GL_MAP_READ_BIT);
    jbyteArray buffer = libglutil_ConvertDataProc(env, thiz, (jbyte*)srcBuff, width, height, type);
    if (srcBuff != NULL) {
        glUnmapBuffer(GL_PIXEL_PACK_BUFFER);
    } else {
        LIBENC_LOGE("GetPboBuffer failure");
    }
    glBindBuffer(GL_PIXEL_PACK_BUFFER, 0);

    return buffer;
}

static JNINativeMethod libglutil_methods[] = {
        {"glUtilReadPixels",   "(IIIII)V",  (void *)libglutil_ReadPixels},
        {"glUtilGetPboBuffer", "(IIII)[B",  (void *)libglutil_GetPboBuffer},
        {"glUtilConvertData",  "([BIII)[B", (void *)libglutil_ConvertData},
};

jint JNI_OnLoad(JavaVM *vm, void *reserved)
{
    gjvm = vm;

    if ((gjenv = getEnv()) == NULL)
    {
        return JNI_ERR;
    }

    gjClass_TextureCapture = (jclass)gjenv->NewGlobalRef(gjenv->FindClass("jp/ne/pickle/libpixelreads/TextureCapture"));
    if (gjClass_TextureCapture == NULL)
    {
        LIBENC_LOGE("Class \"jp/ne/pickle/libpixelreads/TextureCapture\" not found");
        return JNI_ERR;
    }

    if (gjenv->RegisterNatives(gjClass_TextureCapture, libglutil_methods, LIBENC_ARRAY_ELEMS(libglutil_methods)))
    {
        LIBENC_LOGE("methods not registered");
        return JNI_ERR;
    }

    gjMethodId_callNativeMethod = gjenv->GetStaticMethodID(gjClass_TextureCapture, "callNativeMethod", "([BII)V");
    if (gjMethodId_callNativeMethod == NULL)
    {
        LIBENC_LOGE("methods not found");
        return JNI_ERR;
    }

    auto randomClass = gjenv->FindClass("jp/ne/pickle/libpixelreads/UnityConnect");
    jclass classClass = gjenv->GetObjectClass(randomClass);
    auto classLoaderClass = gjenv->FindClass("java/lang/ClassLoader");
    auto getClassLoaderMethod = gjenv->GetMethodID(classClass, "getClassLoader", "()Ljava/lang/ClassLoader;");
    gClassLoader = gjenv->NewGlobalRef(gjenv->CallObjectMethod(randomClass, getClassLoaderMethod));
    gFindClassMethod = gjenv->GetMethodID(classLoaderClass, "findClass", "(Ljava/lang/String;)Ljava/lang/Class;");

    LIBENC_LOGD("JNI_OnLoad");

    return JNI_VERSION_1_6;
}

JNIEnv* getEnv()
{
    JNIEnv *env;
    int status = gjvm->GetEnv((void**)&env, JNI_VERSION_1_6);
    if(status < 0)
    {
        status = gjvm->AttachCurrentThread(&env, NULL);
        if(status < 0)
        {
            LIBENC_LOGE("Env not got");
            return NULL;
        }
    }
    return env;
}

jclass findClass(JNIEnv *env, const char* name)
{
    jclass resultClass = env->FindClass(name);
    jthrowable mException = env->ExceptionOccurred();
    if (mException)
    {
        //env->ExceptionDescribe();
        env->ExceptionClear();
        return static_cast<jclass>(env->CallObjectMethod(gClassLoader, gFindClassMethod, env->NewStringUTF(name)));
    }
    return resultClass;
}
