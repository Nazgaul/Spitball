let audioContext, input, analyser, scriptProcessor;

const createAudioContext = function (elId, myPreferredCameraDeviceId) {
    stopAudioContext();
    navigator.mediaDevices.getUserMedia({audio: myPreferredCameraDeviceId ? {deviceId: myPreferredCameraDeviceId} : true})
        .then(stream => {
        const processInput = audioProcessingEvent => {
            let array = new Uint8Array(analyser.frequencyBinCount);
            analyser.getByteFrequencyData(array);
            let values = 0;

            let length = array.length;
            for (let i = 0; i < length; i++) {
                values += (array[i]);
            }

            let average = values / length;

            //console.log(Math.round(average - 40));
            let micVolume = document.getElementById(`${elId}`);
            if (!micVolume) return;
            micVolume.style.backgroundColor = 'rgba(66, 224, 113, 0.8)';
            micVolume.style.height = '6px';
            micVolume.style.maxWidth = '150px';
            micVolume.style.width = `${Math.round(average)}px`;

        };

        // Handle the incoming audio stream
        audioContext = new (AudioContext || webkitAudioContext)();
        input = audioContext.createMediaStreamSource(stream);
        analyser = audioContext.createAnalyser();
        scriptProcessor = audioContext.createScriptProcessor();

        // Some analyser setup
        analyser.smoothingTimeConstant = 0.3;
        analyser.fftSize = 1024;

        input.connect(analyser);
        analyser.connect(scriptProcessor);
        scriptProcessor.connect(audioContext.destination);
        scriptProcessor.onaudioprocess = processInput;

    }, error => {
        console.log('Something went wrong, or the browser does not support getUserMedia');
        // Something went wrong, or the browser does not support getUserMedia
    });


};
const stopAudioContext = function () {
    if (input) {
        audioContext.close().then(function () {
            console.log('closed audio context');
            // scriptProcessor = null;
            // analyser.disconnect();

        }, (error) => {
            console.log('error stop audio context', error)
        });
    }

};
export default {
    createAudioContext,
    stopAudioContext,
}