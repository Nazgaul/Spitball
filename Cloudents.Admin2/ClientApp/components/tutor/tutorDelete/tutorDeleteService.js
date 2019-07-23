import {connectivityModule} from '../../../services/connectivity.module'

const path = 'AdminTutor/';

const deleteTutor = function(id) {
	return connectivityModule.http.delete(`${path}${id}`);
};

export {
    deleteTutor
}