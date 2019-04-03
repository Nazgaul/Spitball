import {connectivityModule} from "./connectivity.module";
function LeaderBoardItem(ObjInit) {
    this.name = ObjInit.name || '';
    this.score = ObjInit.score || '';
    this.university = ObjInit.university || '';
    this.userId = ObjInit.id || 777;
    this.img = ObjInit.img;
}

function createLeaderBoardItem(ObjInit){
    return new LeaderBoardItem(ObjInit)
}

export default {
    getLeaderBoardItems: (data) => connectivityModule.http.get("/HomePage/LeaderBoard"),
    createLeaderBoardItem
}