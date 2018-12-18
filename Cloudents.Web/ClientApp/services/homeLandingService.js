import { connectivityModule } from "./connectivity.module"
import { LanguageService } from "./language/languageService";

const typeToId = {
    answers: 0,
    sbls: 1,
    money: 2,
    users: 3
};

const typeToTitle = {
    answers: "Total Answers",
    sbls: "SBL Token Traded",
    money: "Money Earned",
    users: "Happy Students"
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