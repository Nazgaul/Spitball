import marketingData from './marketingData';
export default {
    getCampaignData: (name) => {
        if (name &&  marketingData.login[`${name}`]) {
            return marketingData.login[`${name}`];
        } else {
            return marketingData.login['noCampaign'];
        }
    }
}