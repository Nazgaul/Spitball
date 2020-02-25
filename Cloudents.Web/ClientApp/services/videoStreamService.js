import tutorService from "../components/studyroom/tutorService";
const extensionId = 'jaimgihanebafnbcpckdkilkeoomkpik'; // dev && prod
import store from '../store/index.js';
import insightService from './insightService.js'

  function  createVideoSession() {
    console.warn('DEBUG: 22 videoStreamService: createVideoSession')

        const self = this;
        // remove any remote track when joining a new room
        let clearEl = document.getElementById('remoteTrack');
        if (clearEl) {
            clearEl.innerHTML = "";
        }
        self.addDevicesTotrack();
    }
   function enterRoom(){
    console.warn('DEBUG: 23 videoStreamService: enterRoom')

       if (!store.getters['sessionStartClickedOnce']) {
        // if (!!store.getters['accountUser'] && store.getters['getStudyRoomData'].needPayment && !store.getters['getStudyRoomData'].isTutor) {
        //     store.dispatch('requestPaymentURL', {title: 'payme_title', name: store.getters.getStudyRoomData.tutorName});
        //     return;
        // }
        //leave this action here so that people that fills the 'pay me' wont get a loading button
        console.warn('DEBUG: 44 videoStreamService: setSesionClickedOnce,true before ')
        
        store.dispatch('setSesionClickedOnce', true);
        console.warn('DEBUG: 44.1 videoStreamService: setSesionClickedOnce,true after ')

            // store.dispatch('setSesionClickedOnce', true);
            if (store.getters['getStudyRoomData'].isTutor) {
                console.warn('DEBUG: 44.2 videoStreamService: (store.getters[getStudyRoomData].isTutor) ')
                
                console.warn('DEBUG: 44.3 videoStreamService: updateCurrentRoomState before')
                store.dispatch('updateCurrentRoomState', 'loading');
                console.warn('DEBUG: 44.4 videoStreamService: updateCurrentRoomState after')

                
                console.warn('DEBUG: 44.5 videoStreamService: tutorService.enterRoom(store.getters[getRoomId]) before')
                tutorService.enterRoom(store.getters['getRoomId']).then(() => {
                    console.warn('DEBUG: 44.6 videoStreamService: tutorService.enterRoom(store.getters[getRoomId]) then')

                    setTimeout(() => {
                        console.warn('DEBUG: 44.7 videoStreamService: tutorService.enterRoom(store.getters[getRoomId]) Settimeout')

                        this.createVideoSession();
                        console.warn('DEBUG: 44.8 videoStreamService: tutorService.enterRoom(store.getters[getRoomId]) Settimeout after')

                    }, 1000);
                });
            } else {
                this.createVideoSession();
            }
        }
    }

    function createTwillioTracks(audioDevice, videoDevice){
        console.warn('DEBUG: 24 videoStreamService: createTwillioTracks')

        const token = store.getters['getJwtToken']; //get jwt from store
        let tracksCreated = [];
        !!videoDevice ? tracksCreated.push(videoDevice) : '';
        !!audioDevice ? tracksCreated.push(audioDevice) : '';
        tracksCreated.push(tutorService.dataTrack);
        let connectOptions = {
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
        }
    }

   async function addDevicesTotrack(){
    console.warn('DEBUG: 25 videoStreamService: addDevicesTotrack')

        let audioSetObj = store.getters['getLocalAudioTrack'];
        let videoSetObj = store.getters['getLocalVideoTrack'];
        let isVideoActive = store.getters['getIsVideoActive'];
        let isAudioActive = store.getters['getIsAudioActive'];
        let audioDevice = audioSetObj && isAudioActive ? audioSetObj : false;
        let videoDevice = videoSetObj && isVideoActive ? videoSetObj : false;
        createTwillioTracks(audioDevice, videoDevice);
    }

    //get try to get share stream via chrome extension
   function getUserScreen() {
    console.warn('DEBUG: 26 videoStreamService: getUserScreen')

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
            insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoStream_notBrowser', err);
            return Promise.reject("notBrowser");
        }
    }
export default {
    extensionId,
    getUserScreen,
    addDevicesTotrack,
    enterRoom,
    createVideoSession
}