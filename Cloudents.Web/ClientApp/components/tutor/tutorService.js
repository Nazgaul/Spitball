import Twilio, { LocalDataTrack } from 'twilio-video';
import { connectivityModule } from '../../services/connectivity.module';


const dataTrack = new LocalDataTrack();

const uploadCanvasImage = function(formData){
    return connectivityModule.http.post("Tutoring/upload", formData);
}


export {
    dataTrack,
    uploadCanvasImage
}