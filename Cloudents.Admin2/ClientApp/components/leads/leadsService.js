import { connectivityModule } from '../../services/connectivity.module';

const path = 'AdminLead/';

function AdminLeads(objInit) {
    this.course = objInit.course;
    this.email = objInit.email;
    this.id = objInit.id;
    this.name = objInit.name;
    this.phone = objInit.phone;
    this.text = objInit.text;
    this.university = objInit.university;
    this.dateTime = new Date( objInit.dateTime);
}

function createAdminLead(objInit) {
    return new AdminLeads(objInit);
}

const getAdminLeads = () => {
    return connectivityModule.http.get(`${path}`).then(adminLeads => {
        let adminLeadsArr = [];
        adminLeads.forEach(leads => {
            adminLeadsArr.push(createAdminLead(leads));
        });
        return adminLeadsArr;
    });
};

export {
    getAdminLeads
}