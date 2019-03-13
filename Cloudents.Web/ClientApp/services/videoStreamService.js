// const extensionId = 'dhndcoampgbambhkkjkicnibhbndjaop'; // localhost spitball share
 const extensionId = 'hicolpoppnllddloocbcjfeoijgjfdeg'; // dev && prod
//  const extensionId = 'chombcfbjenobkieohgkjlmmhehfgomf'; // twillio

export default {

    //get/try to get share stream via chrome extension
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
                const request = { sources: ['window', 'screen', 'tab'] };
                chrome.runtime.sendMessage(extensionId, request, response => {
                    if(!response){
                        let error = 'noExtension';
                        console.log('Extension not installed');
                        reject(error);
                    }
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
}