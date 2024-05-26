interface IAudio {
    isPlaying() : boolean;
    start() : void;
    stop() : void;
}

class AudioPlayer implements IAudio {
    private context : AudioContext;
    private oscillator : OscillatorNode;
    private isSoundPlaying : boolean = false;

    isPlaying(): boolean {
        return this.isSoundPlaying;
    }

    start() : void {
        this.context = new AudioContext()
        this.oscillator = this.context.createOscillator()
        this.oscillator.type = "sine"
        this.oscillator.connect(this.context.destination)
        this.oscillator.start()
        this.isSoundPlaying = true;
    }

    stop() : void {
        if(this.isSoundPlaying === false) return;
        
        this.isSoundPlaying = false;
        this.oscillator.stop();
        this.context.close();
    }
}
