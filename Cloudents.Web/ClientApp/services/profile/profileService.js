import profileData from './profileData';

export default {
    getProfileData: (name) => {
        if (name &&  profileData[`${name}`]) {
            return profileData[`${name}`];
        } else {
            return profileData['profileGeneral'];
        }
    }
}