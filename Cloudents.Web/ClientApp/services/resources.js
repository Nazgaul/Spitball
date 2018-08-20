
export const transformLocation = (params) => {
    let location = params.location;
    delete params.location;
    if (location) {
        params['location.point.latitude'] = location.latitude;
        params['location.point.longitude'] = location.longitude;
    }
    return params;
};

