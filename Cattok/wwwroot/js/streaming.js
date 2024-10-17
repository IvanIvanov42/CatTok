window.streamingFunctions = {
    streaming: null,
    peerConnections: {}, // connectionId: RTCPeerConnection
    config: {
        iceServers: [{ urls: 'stun:stun.l.google.com:19302' }]
    },
    dotNetRef: null,

    setDotNetReference(dotNetRef) {
        this.dotNetRef = dotNetRef;
    },

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

        // Close all peer connections
        for (const connectionId in this.peerConnections) {
            const pc = this.peerConnections[connectionId];
            pc.close();
            delete this.peerConnections[connectionId];
        }

        const livestreamVideo = document.getElementById('livestreamVideo');
        if (livestreamVideo) {
            livestreamVideo.srcObject = null;
            livestreamVideo.style.display = 'none';
        }
    },

    clearRemoteVideo() {
        const remoteVideo = document.getElementById('remoteVideo');
        if (remoteVideo) {
            remoteVideo.srcObject = null;
        }

        // Close all peer connections
        for (const connectionId in this.peerConnections) {
            const pc = this.peerConnections[connectionId];
            pc.close();
            delete this.peerConnections[connectionId];
        }
    },

    async createOffer(connectionId) {
        const pc = this.initializePeerConnection(connectionId);
        this.streaming.getTracks().forEach(track => pc.addTrack(track, this.streaming));

        try {
            const offer = await pc.createOffer();
            await pc.setLocalDescription(offer);
            return JSON.stringify(offer);
        } catch (error) {
            console.log(error);
            return null;
        }
    },

    async receiveAnswer(connectionId, answer) {
        const pc = this.peerConnections[connectionId];
        let remoteDesc = new RTCSessionDescription(JSON.parse(answer));
        await pc.setRemoteDescription(remoteDesc);

        if (pc._queuedIceCandidates) {
            for (const candidate of pc._queuedIceCandidates) {
                await pc.addIceCandidate(candidate);
            }
            pc._queuedIceCandidates = null;
        }
    },

    async addIceCandidate(connectionId, candidate) {
        const pc = this.peerConnections[connectionId];
        let iceCandidate = new RTCIceCandidate(JSON.parse(candidate));
        if (pc.remoteDescription && pc.remoteDescription.type) {
            console.log(`Adding ICE candidate for connectionId: ${connectionId}`);
            await pc.addIceCandidate(iceCandidate);
        } else {
            if (!pc._queuedIceCandidates) {
                pc._queuedIceCandidates = [];
            }
            pc._queuedIceCandidates.push(iceCandidate);
        }
    },

    async receiveOffer(streamerConnectionId, offer) {
        try {
            const pc = this.initializePeerConnection(streamerConnectionId);
            pc.ontrack = event => this.attachRemoteStream(event.streams[0]);

            const remoteDesc = new RTCSessionDescription(JSON.parse(offer));
            await pc.setRemoteDescription(remoteDesc);

            if (pc._queuedIceCandidates) {
                for (const candidate of pc._queuedIceCandidates) {
                    await pc.addIceCandidate(candidate);
                }
                pc._queuedIceCandidates = null;
            }

            const answer = await pc.createAnswer();
            await pc.setLocalDescription(answer);

            await this.dotNetRef.invokeMethodAsync('SendAnswerToServer', streamerConnectionId, JSON.stringify(answer));
        } catch (error) {
            console.log(`Error in receiveOffer: ${error}`);
        }
    },

    initializePeerConnection(connectionId) {
        const pc = new RTCPeerConnection(this.config);
        this.peerConnections[connectionId] = pc;

        pc.onicecandidate = event => {
            if (event.candidate) {
                this.dotNetRef.invokeMethodAsync('SendIceCandidate', connectionId, JSON.stringify(event.candidate));
            }
        };

        return pc;
    },

    attachRemoteStream(stream) {
        const remoteVideo = document.getElementById('remoteVideo');
        if (!remoteVideo) {
            console.warn('Remote video element not found in DOM.');
            return;
        }

        remoteVideo.srcObject = stream;
    }
};
