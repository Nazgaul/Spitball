import tutorService from "../components/tutor/tutorService";
const extensionId = 'jaimgihanebafnbcpckdkilkeoomkpik'; // dev && prod
import store from '../store/index.js';
import { createLocalTracks } from 'twilio-video';

export default {
   async getAvalHardware(){
        let self = this;
        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
            console.log("enumerateDevices() not supported.");
            return;
        }
        const token = store.getters['getJwtToken']; //get jwt from store
        // List cameras and microphones.
        let devices = await navigator.mediaDevices.enumerateDevices();
        //navigator.mediaDevices.enumerateDevices()
        //.then(function (devices) {
        devices.forEach(function (device) {
            console.log(device.kind + ": " + device.label +
                " id = " + device.deviceId);
            self.availableDevices.push(device.kind);
        });
        let connectOptions;
        //create local track with custom names
        let audioTrackName = `audio_${store.getters['getStudyRoomData'].isTutor ? 'tutor' : 'student'}_${store.getters['accountUser'].id}`;
        let videoTrackName = `video_${store.getters['getStudyRoomData'].isTutor ? 'tutor' : 'student'}_${store.getters['accountUser'].id}`;
        let audioSetObj = {
            audio: self.availableDevices.includes('audioinput'),
            name: audioTrackName
        };
        let videoSetObj = {
            video: self.availableDevices.includes('videoinput'),
            name: videoTrackName
        };
        let audioDevice = await navigator.mediaDevices.getUserMedia({ audio: true }).then(y => audioSetObj, z => false);
        let videoDevice = await navigator.mediaDevices.getUserMedia({ video: true }).then(y => videoSetObj, z => false);
        createLocalTracks({
                              audio: audioDevice,
                              video:videoDevice
                          }).then((tracksCreated) => {
            let localMediaContainer = document.getElementById('localTrack');
            tracksCreated.forEach((track) => {
                localMediaContainer.innerHTML = "";
                localMediaContainer.appendChild(track.attach());
                self.localTrackAval = true;
            });
            tracksCreated.push(tutorService.dataTrack);
            connectOptions = {
                tracks: tracksCreated,
                networkQuality: true
            };
            tutorService.connectToRoom(token, connectOptions);
            if (!store.getters['getStudyRoomData'].isTutor) {
                store.dispatch('updateCurrentRoomState', store.state.tutoringMainStore.roomStateEnum.active);
            }

        }, (error) => {
            store.dispatch('updateToasterParams', {
                toasterText: "We having trouble connection you to the room",
                showToaster: true,
                toasterType: 'error-toaster'
            });

        });
    },
    extensionId,
    //get try to get share stream via chrome extension
    getUserScreen() {
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
    },
}