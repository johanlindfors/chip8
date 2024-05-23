interface IAudio {
    isPlaying() : boolean;
    start() : void;
    stop() : void;
}

class AudioPlayer implements IAudio {
    isPlaying(): boolean {
        return false;
    }

    start() : void {

    }

    stop() : void {
        
    }
}