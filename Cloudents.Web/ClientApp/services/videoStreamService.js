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
       if (!store.getters['sessionStartClickedOnce']) {
        if (!!store.getters['accountUser'] && store.getters['getStudyRoomData'].needPayment && !store.getters['getStudyRoomData'].isTutor) {
            walletService.getPaymeLink().then(({ data }) => {
                store.dispatch('updatePaymentUrl', data.link);
                store.dispatch('updatePaymentDialog',true)
            });
            return;
        }
        //leave this action here so that people that fills the 'pay me' wont get a loading button
        store.dispatch('setSesionClickedOnce', true);
        //if blocked or not available  use of media devices do not allow session start
        if (store.getters['getNotAllowedDevices'] && store.getters['getNotAvaliableDevices'] ) {
            store.dispatch('updateTestDialogState', true);
            return;
        }
        
            if (store.getters['getStudyRoomData'].isTutor) {
                store.dispatch('updateCurrentRoomState', 'loading');
                tutorService.enterRoom(store.getters['getRoomId']).then(() => {
                    setTimeout(() => {
                        this.createVideoSession();
                    }, 1000);
                });
            } else {
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
            for(let track of tracksCreated){
                if(track.kind === 'audio'){
                    store.commit('setLocalAudioTrack',track)                    
                }
                if(track.kind === 'video'){
                    store.commit('setLocalVideoTrack',track)
                }
            }
            // let localMediaContainer = document.getElementById('localTrack');
            //clear before attach
            // localMediaContainer.innerHTML = "";
            //attach tracks
            // tutorService.attachTracks(tracksCreated, localMediaContainer);
            self.localTrackAval = true;
            //add datatrack, after created audio and or video tracks
            tracksCreated.push(tutorService.dataTrack);
            connectOptions = {
                logLevel :'debug',
                tracks: tracksCreated,
                networkQuality: {
                    local: 3, // LocalParticipant's Network Quality verbosity [1 - 3]
                    remote: 3 // RemoteParticipants' Network Quality verbosity [0 - 3]
                  }
            };
            tutorService.connectToRoom(token, connectOptions);
            if (!store.getters['getStudyRoomData'].isTutor) {
                store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.active);
                // store.dispatch('setSesionClickedOnce', false)
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
        availableDevices.length = 0;
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
            return Promise.reject("notBrowser");
        }
        // if (chrome.runtime) {
        //     return new Promise((resolve, reject) => {
        //         const request = { sources: ['window', 'screen', 'tab'] };
        //         chrome.runtime.sendMessage(extensionId, request, response => {
        //             //none installed return error string code
        //             if (!response) {
        //                 let error = 'noExtension';
        //                 console.log('Extension not installed');
        //                 reject(error);
        //             }
        //             if (response && response.type === 'success') {
        //                 resolve({ streamId: response.streamId });
        //             } else {
        //                 reject(new Error('Could not get stream'));
        //             }
        //         });
        //     }).then(async response => {
        //         const stream = await navigator.mediaDevices.getUserMedia({
        //             video: {
        //                 mandatory: {
        //                     chromeMediaSource: 'desktop',
        //                     chromeMediaSourceId: response.streamId
        //                 }
        //             }
        //         });
        //         return stream.getVideoTracks()[0];
        //     });
        // }
        
    }
export default {
    extensionId,
    getUserScreen,
    addDevicesTotrack,
    enterRoom,
    createVideoSession
}