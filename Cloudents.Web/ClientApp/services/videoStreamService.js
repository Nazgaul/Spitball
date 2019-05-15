// const extensionId = 'dhndcoampgbambhkkjkicnibhbndjaop'; // localhost spitball share
const extensionId = 'jaimgihanebafnbcpckdkilkeoomkpik'; // dev && prod
//  const extensionId = 'chombcfbjenobkieohgkjlmmhehfgomf'; // twillio

export default {
    extensionId,
    //get/try to get share stream via chrome extension
    getUserScreen() {
        function isFirefox() {
            let mediaSourceSupport = !!navigator.mediaDevices.getSupportedConstraints().mediaSource;
            let matchData = navigator.userAgent.match('Firefox\/([0-9]+)\.');
            let firefoxVersion = 0;
            if (matchData && matchData[1]) {
                firefoxVersion = parseInt(matchData[1], 10);
            }
            return mediaSourceSupport && firefoxVersion >= 52;
        }

        function isChrome() {
            return 'chrome' in window;
        }
        function getChromeVersion() {
            var raw = navigator.userAgent.match(/Chrom(e|ium)\/([0-9]+)\./);

            return raw ? parseInt(raw[2], 10) : false;
        }

        function canScreenShare() {
            return isFirefox() || isChrome();
        }

        if (!canScreenShare()) {
            return Promise.reject("not supported browser");
        }
        if (isChrome()) {

            if (getChromeVersion() > 72) {
                //return navigator.mediaDevices.getUserMedia();
                return navigator.mediaDevices.getDisplayMedia().then(stream => {
                    return stream.getTracks()[0];

                });

            }
            return new Promise((resolve, reject) => {
                const request = { sources: ['window', 'screen', 'tab'] };
                chrome.runtime.sendMessage(extensionId, request, response => {
                    //none installed return error string code
                    if (!response) {
                        let error = 'noExtension';
                        console.log('Extension not installed');
                        reject(error);
                    }
                    if (response && response.type === 'success') {
                        resolve({ streamId: response.streamId });
                    } else {
                        reject(new Error('Could not get stream'));
                    }
                });
            }).then(async response => {
                const stream = await navigator.mediaDevices.getUserMedia({
                    video: {
                        mandatory: {
                            chromeMediaSource: 'desktop',
                            chromeMediaSourceId: response.streamId
                        }
                    }
                });
                return stream.getVideoTracks()[0];
            });
        } else if (isFirefox()) {

            let displayMediaOptions = {
                video: {
                    cursor: "never"
                },
                audio: false
            };
            return navigator.mediaDevices.getDisplayMedia(displayMediaOptions).then(stream => {

                return stream.getTracks()[0];
            });
        }
    },
}