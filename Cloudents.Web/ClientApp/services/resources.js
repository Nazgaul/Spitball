
export const transformLocation = (params) => {
    let location = params.location;
    delete params.location;
    if (location) {
        params['location.latitude'] = location.latitude;
        params['location.longitude'] = location.longitude;
    }
    return params;
};

