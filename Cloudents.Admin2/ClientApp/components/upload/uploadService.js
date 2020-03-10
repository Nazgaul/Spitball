import {connectivityModule} from '../../services/connectivity.module'

const path = 'AdminUpload/';

const getBlobs = function(id) {
	return connectivityModule.http.get(path).then((urls) => {
        return urls
    })
};

export {
    getBlobs
}