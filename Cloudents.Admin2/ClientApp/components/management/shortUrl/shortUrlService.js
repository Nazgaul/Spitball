import { connectivityModule } from '../../../services/connectivity.module';


const addUrl = function (item) {
    return connectivityModule.http.post(`AdminShortUrl/url`, { "Destination": item.destination, "Identifier": item.identifier, "Expiration": item.date })
        .then((resp) => {
            return Promise.resolve(resp);
        }, (error) => {
            return Promise.reject(error);
        });
};

export
{
    addUrl
}