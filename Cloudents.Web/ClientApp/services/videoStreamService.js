import tutorService from "../components/tutor/tutorService";
const extensionId = 'jaimgihanebafnbcpckdkilkeoomkpik'; // dev && prod
import store from '../store/index.js';
import { createLocalTracks } from 'twilio-video';
import walletService from "./walletService";
let availableDevices = [];

  function  createVideoSession() {
        const self = this;
        // remove any remote track when joining a new room
        let clearEl = document.getElementById('remoteTrack');
        if (clearEl) {
            clearEl.innerHTML = "";
        }
        self.addDevicesTotrack();
    }
   function enterRoom(){
       store.dispatch('setSesionClickedOnce', true);
        if (!!store.getters['accountUser'] && store.getters['accountUser'].needPayment && !store.getters['getStudyRoomData'].isTutor) {
            walletService.getPaymeLink().then(({ data }) => {
                global.open(data.link, '_blank', 'height=520,width=440');
            });
            return;
        }
        //if blocked or not available  use of media devices do not allow session start
        if (store.getters['getNotAllowedDevices'] && store.getters['getNotAvaliableDevices'] ) {
            store.dispatch('updateTestDialogState', true);
            return;
        }
        if (!store.getters['sessionStartClickedOnce']) {
            // store.dispatch('setSesionClickedOnce', true);
            if (store.getters['getStudyRoomData'].isTutor) {
                store.dispatch('updateCurrentRoomState', 'loading');
                tutorService.enterRoom(store.getters['getRoomId']).then(() => {
                    setTimeout(() => {
                        this.createVideoSession();
                    }, 1000);
                });
            } else {
                //join
                // store.dispatch('setSesionClickedOnce', true);
                this.createVideoSession();
            }
        }
    }

    function createTwillioTracks(audioDevice, videoDevice){
        let connectOptions;
        const token = store.getters['getJwtToken']; //get jwt from store
        createLocalTracks({
                              audio: audioDevice,
                              video:videoDevice
                          }).then((tracksCreated) => {
            let localMediaContainer = document.getElementById('localTrack');
            //clear before attach
            localMediaContainer.innerHTML = "";
            //attach tracks
            tutorService.attachTracks(tracksCreated, localMediaContainer);
            self.localTrackAval = true;
            //add datatrack, after created audio and or video tracks
            tracksCreated.push(tutorService.dataTrack);
            connectOptions = {
                tracks: tracksCreated,
                networkQuality: true
            };
            tutorService.connectToRoom(token, connectOptions);
            if (!store.getters['getStudyRoomData'].isTutor) {
                store.dispatch('updateCurrentRoomState', store.state.tutoringMainStore.roomStateEnum.active);
                store.dispatch('setSesionClickedOnce', false)
            }

        }, (error) => {
            store.dispatch('updateToasterParams', {
                toasterText: "We having trouble connection you to the room",
                showToaster: true,
                toasterType: 'error-toaster'
            });
            store.dispatch('setSesionClickedOnce', false)

        });
    }

   async function addDevicesTotrack(){
        let self = this;
        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
            console.log("enumerateDevices() not supported.");
            return;
        }

        // List cameras and microphones.
        let devices = await navigator.mediaDevices.enumerateDevices();
        devices.forEach(function (device) {
            console.log(device.kind + ": " + device.label +
                " id = " + device.deviceId);
            availableDevices.push(device.kind);
        });
        //create local track with custom names
        let audioTrackName = `audio_${store.getters['getStudyRoomData'].isTutor ? 'tutor' : 'student'}_${store.getters['accountUser'].id}`;
        let videoTrackName = `video_${store.getters['getStudyRoomData'].isTutor ? 'tutor' : 'student'}_${store.getters['accountUser'].id}`;
        let audioSetObj = {
            audio: availableDevices.includes('audioinput'),
            name: audioTrackName
        };
        let videoSetObj = {
            video: availableDevices.includes('videoinput'),
            name: videoTrackName
        };
        let audioDevice = await navigator.mediaDevices.getUserMedia({ audio: true }).then(y => audioSetObj, z => false);
        let videoDevice = await navigator.mediaDevices.getUserMedia({ video: true }).then(y => videoSetObj, z => false);
       createTwillioTracks(audioDevice, videoDevice);
    }

    //get try to get share stream via chrome extension
   function getUserScreen() {
        let displayMediaOptions = {
            video:true,
            audio: false
        };
        try {
        return navigator.mediaDevices.getDisplayMedia(displayMediaOptions).then(stream => {
            return stream.getTracks()[0];
        });
        }
        catch(err) {
        }
        if (chrome.runtime) {
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
        }
        return Promise.reject("notBrowser");
    }
export default {
    extensionId,
    getUserScreen,
    addDevicesTotrack,
    enterRoom,
    createVideoSession
}