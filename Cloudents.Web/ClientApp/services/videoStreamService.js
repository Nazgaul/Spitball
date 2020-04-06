import tutorService from "../components/studyroom/tutorService";
import studyRoomService from "../services/studyRoomService.js";
const extensionId = 'jaimgihanebafnbcpckdkilkeoomkpik'; // dev && prod
import store from '../store/index.js';
import insightService from './insightService.js'

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
        //leave this action here so that people that fills the 'pay me' wont get a loading button
        store.dispatch('setSesionClickedOnce', true);
            if (store.getters['getStudyRoomData'].isTutor) {
                store.dispatch('updateCurrentRoomState', 'loading');
                studyRoomService.enterRoom(store.getters['getRoomId']).then(() => {
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

        createTwillioTracks();
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