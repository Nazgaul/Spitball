import { connectivityModule } from "./connectivity.module"
import { LanguageService } from "./language/languageService";

const typeToId = {
    answers: 0,
    sbls: 1,
    money: 2,
    users: 3
};

const typeToTitle = {
    // answers:  LanguageService.getValueByKey("landingPage_stats_answers"),
    // sbls: LanguageService.getValueByKey("landingPage_stats_sbls"),
    // money: LanguageService.getValueByKey("landingPage_stats_money"),
    // users:  LanguageService.getValueByKey("landingPage_stats_users"),
};

function statisticsData(key, val){
    this.id = typeToId[key];
    this.title = typeToTitle[key];
    this.data = val;
}

function statisticsObj(ObjInit){
    this.users = ObjInit.users;
    this.answers = ObjInit.answers;
    this.sbls = ObjInit.sbls;
    this.money = ObjInit.sbls;
}

function createStatisticsData(ObjInit){
    let serverConverter = new statisticsObj(ObjInit);
    let statisticsDataResult = [];
    for(let prop in serverConverter){
        let stat = new statisticsData(prop, serverConverter[prop]);
        statisticsDataResult.push(stat)
    }
    //sort objects by id to keep the requested order of items
    statisticsDataResult.sort(function(a, b) {
        return  ( a.id - b.id  ||  a.title.localeCompare(b.title) );
    });
    return statisticsDataResult;
}


export default {
    getStatistics: () => connectivityModule.http.get("/HomePage"),
    createStatisticsData,
}