import { connectivityModule } from '../../../services/connectivity.module'

function ActiveUserItem(objInit) {
    this.userId = objInit.id;
    this.country = objInit.country;
    this.flags = objInit.flags;
}

function createActiveUserItem(objInit) {
    return new ActiveUserItem(objInit);
}

const getActiveUsers = function (minFlags, page) {
    let path = `AdminUser/usersFlags?minFlags=${minFlags}&page=${page}`;
    return connectivityModule.http.get(path).then((data) => {
        let arrActiveUsers = [];
        if (data.flags.length > 0) {
            data.flags.forEach((activeItem) => {
               arrActiveUsers.push(createActiveUserItem(activeItem));
            });
        }

        let objToReturn = {
            flags: arrActiveUsers,
            rows:  data.rows
        };
        return Promise.resolve(objToReturn);
    }, (err) => {
        return Promise.reject(err);
    });
};

export {
    getActiveUsers,
    createActiveUserItem,
}
