package com.self.viewtoglrendering;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.util.AttributeSet;
import android.util.Log;
import android.webkit.WebView;

import jp.ne.pickle.libpixelreads.TextureCapture;

public class GLWebView extends WebView implements GLRenderable{

    private ViewToGLRenderer mViewToGLRenderer;

    // default constructors

    public GLWebView(Context context) {
        super(context);
    }

    public GLWebView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public GLWebView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    // draw magic
    @Override
    public void draw( Canvas canvas ) {
        if (mViewToGLRenderer != null) {
            //returns canvas attached to gl texture to draw on
            Canvas glAttachedCanvas = mViewToGLRenderer.onDrawViewBegin();
            if(glAttachedCanvas != null) {
                //translate canvas to reflect view scrolling
                float xScale = glAttachedCanvas.getWidth() / (float)canvas.getWidth();
                glAttachedCanvas.scale(xScale, xScale);
                glAttachedCanvas.translate(-getScrollX(), -getScrollY());
                //draw the view to provided canvas
                super.draw(glAttachedCanvas);
            }
            // notify the canvas is updated
            mViewToGLRenderer.onDrawViewEnd();
        } else {
            super.draw(canvas);
        }
    }

    public void setViewToGLRenderer(ViewToGLRenderer viewTOGLRenderer){
        mViewToGLRenderer = viewTOGLRenderer;
    }
}
