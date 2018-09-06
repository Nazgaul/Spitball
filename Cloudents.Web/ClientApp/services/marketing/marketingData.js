import { LanguageService } from "../language/languageService";

const marketingData = {
    login: {
        noCampaign: {
            stepOne: {
                text: LanguageService.getValueByKey("marketingData_login_noCampaign_stepOne"),

            },
            stepTwo: {
                text: LanguageService.getValueByKey("marketingData_login_noCampaign_stepTwo"),
            }
        },
        askaquestion: {
            stepOne: {
                text: LanguageService.getValueByKey("marketingData_login_askQuestion_stepOne"),
            },
            stepTwo: {
                text: LanguageService.getValueByKey("marketingData_login_askQuestion_stepTwo"),
            }
        },
   }
};
export default marketingData