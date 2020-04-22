let audioContext, input, analyser, scriptProcessor;

const createAudioContext = function (elId, myPreferredCameraDeviceId) {
    stopAudioContext();
    navigator.mediaDevices.getUserMedia({audio: myPreferredCameraDeviceId ? {deviceId: myPreferredCameraDeviceId} : true})
        .then(stream => {
        const processInput = () => {
            let array = new Uint8Array(analyser.frequencyBinCount);
            analyser.getByteFrequencyData(array);
            let values = 0;

            let length = array.length;
            let i;
            for (i = 0; i < length; i++) {
                values += (array[i]);
            }

            let average = values / length;

            let micVolume = document.getElementById(`${elId}`);
            if (!micVolume) return;
            micVolume.style.backgroundColor = '#16eab1';
            micVolume.style.height = '6px';
            micVolume.style.borderRadius = '2px';
            micVolume.style.maxWidth = '40px';
            micVolume.style.width = `${Math.round(average)}px`;

        };

        // Handle the incoming audio stream
        /* global webkitAudioContext */
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

    }, () => {
        console.log('Something went wrong, or the browser does not support getUserMedia');
    });


};
const stopAudioContext = function () {
    if (input) {
        audioContext.close().then(function () {
            console.log('closed audio context');
        }, (error) => {
            console.log('error stop audio context', error);
        });
    }

};
export default {
    createAudioContext,
    stopAudioContext,
}