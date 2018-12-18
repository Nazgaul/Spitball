import { connectivityModule } from "./connectivity.module"
import { LanguageService } from "./language/languageService";

const typeToId = {
    answers: 0,
    sbls: 1,
    money: 2,
    users: 3
};

const typeToTitle = {
    answers:  LanguageService.getValueByKey("landingPage_stats_answers"),
    sbls: LanguageService.getValueByKey("landingPage_stats_sbls"),
    money: LanguageService.getValueByKey("landingPage_stats_money"),
    users:  LanguageService.getValueByKey("landingPage_stats_users"),
};

function statisticsData(key, val){
    this.id = typeToId[key];
    this.title = typeToTitle[key];
    this.data = val;
}

function createStatisticsData(ObjInit){
    let statisticsDataResult = [];
    for(let prop in ObjInit){
        let stat = new statisticsData(prop, ObjInit[prop]);
        statisticsDataResult.push(stat)
    }
    return statisticsDataResult;
}


export default {
    getStatistics: () => connectivityModule.http.get("/HomePage"),
    createStatisticsData,
}