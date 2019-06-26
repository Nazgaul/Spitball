import { connectivityModule } from '../../../services/connectivity.module';

function ShortHandUrl(objInit) {
    this.identifier = objInit.identifier;
    this.destination = objInit.destination;
    this.expiration = objInit.expiration ? new Date(objInit.expiration).toISOString().substr(0, 10) : 'indefinite'
}

function createShortHandUrl(objInit) {
    return new ShortHandUrl(objInit);
}

const addUrl = function (item) {
    return connectivityModule.http.post(`AdminShortUrl/url`, { "destination": item.destination, "identifier": item.identifier, "expiration": item.date })
        .then((resp) => {
            return createShortHandUrl(resp);
        }, (error) => {
            return error;
        });
};

export {
    addUrl
}