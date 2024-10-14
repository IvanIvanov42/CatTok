window.streamingFunctions = {
    streaming: null,

    async startStreaming() {
        try {
            this.streaming = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
            this.attachStream();
            return true;
        } catch (error) {
            console.error('Couldn\'t access media devices.', error);
            alert('Unable to access camera or microphone.');
            return false;
        }
    },

    attachStream() {
        const livestreamVideo = document.getElementById('livestreamVideo');
        if (this.streaming && livestreamVideo) {
            livestreamVideo.srcObject = this.streaming;
            livestreamVideo.style.display = 'block';
        }
    },

    stopStreaming() {
        if (this.streaming) {
            this.streaming.getTracks().forEach(track => track.stop());
            this.streaming = null;
        }
        const livestreamVideo = document.getElementById('livestreamVideo');
        if (livestreamVideo) {
            livestreamVideo.srcObject = null;
            livestreamVideo.style.display = 'none';
        }
    }
};
