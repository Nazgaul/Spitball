import { connectivityModule } from "./connectivity.module";
// const extensionId = 'dhndcoampgbambhkkjkicnibhbndjaop'; // localhost spitball share
// const extensionId = 'chombcfbjenobkieohgkjlmmhehfgomf'; // localhost TWillio ext
const extensionId = 'hicolpoppnllddloocbcjfeoijgjfdeg'; // dev && prod

export default {
    generateRoom: () => {
        return connectivityModule.http.post("tutoring/create");
    },
    //get/try to get share stream
    getUserScreen() {
        function isFirefox() {
            let mediaSourceSupport = !!navigator.mediaDevices.getSupportedConstraints().mediaSource;
            let matchData = navigator.userAgent.match('/Firefox/(d) /');
            let firefoxVersion = 0;
            if (matchData && matchData[1]) {
                firefoxVersion = parseInt(matchData[1], 10);
            }
            return mediaSourceSupport && firefoxVersion >= 52;
        }

        function isChrome() {
            return 'chrome' in window;
        }

        function canScreenShare() {
            return isFirefox() || isChrome();
        }

        if (!canScreenShare()) {
            return;
        }
        if (isChrome()) {
            return new Promise((resolve, reject) => {
                const request = {
                    sources: ['screen']
                };
                chrome.runtime.sendMessage(extensionId, request, response => {
                    if (response && response.type === 'success') {
                        resolve({streamId: response.streamId});
                    } else {
                        reject(new Error('Could not get stream'));
                    }
                });
            }).then(response => {
                return navigator.mediaDevices.getUserMedia({
                    video: {
                        mandatory: {
                            chromeMediaSource: 'desktop',
                            chromeMediaSourceId: response.streamId
                        }
                    }
                });
            });
        } else if (isFirefox()) {
            return navigator.mediaDevices.getUserMedia({
                video: {
                    mediaSource: 'screen'
                }
            });
        }
    },
    // Token get
    getToken: (name, identityName) => {
        let userIdentity = identityName || '';
        return connectivityModule.http.get(`tutoring/join?roomName=${name}&identityName=${userIdentity}`)
            .then((data) => {
                return data.data.token
            });
    }

}