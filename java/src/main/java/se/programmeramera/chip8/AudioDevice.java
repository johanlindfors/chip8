package se.programmeramera.chip8;

import javax.sound.sampled.*;

public class AudioDevice implements Audio {

    public static float SAMPLE_RATE = 44000f;

    private AudioFormat af;
    private SourceDataLine sdl;
    private boolean isPlaying = false;
    private byte[] buf;
    private Thread playThread;

    public AudioDevice() {
        super();

        buf = new byte[256];
        af = new AudioFormat(
            SAMPLE_RATE,        // sampleRate
            8, // sampleSizeInBits
            1,         // channels
            true,        // signed
            false);   // bigEndian
        try {
            sdl = AudioSystem.getSourceDataLine(af);
            sdl.open();

            // generate the sound to play
            for (int i=0; i<buf.length; i++)
                buf[i] = 121;

            for (int i=buf.length/3; i<2*buf.length/3; i++)
                buf[i] = (byte)255-121;
        } catch (LineUnavailableException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void start() {
        if (isPlaying) return;
        
        isPlaying = true;
        playThread = new PlayThread();
        playThread.setPriority(Thread.MAX_PRIORITY);
        playThread.start();
    }

    @Override
    public void stop() {
        isPlaying = false;
    }

    @Override
    public boolean isPlaying() {
        return isPlaying;
    }
    
    class PlayThread extends Thread {
        public void run(){
            try {
                sdl.start();
                do {
                    sdl.write(buf, 0, buf.length);
                } while (isPlaying);
                sdl.stop();
                sdl.flush();
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
    }
}
