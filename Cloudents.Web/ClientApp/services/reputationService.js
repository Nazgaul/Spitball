import { connectivityModule } from "./connectivity.module"

export default {
    calculateRankByScore: (score) => {
        const scoreRange= {
            first: 4,
            second: 10,
            third: 20
        }
        if (score <= scoreRange.first) {
            return 0;
        } else if (score > scoreRange.first && score <= scoreRange.second) {
            return 1;
        } else if (score > scoreRange.second && score <= scoreRange.third) {
            return 2;
        } else if (score > scoreRange.third) {
            return 3;
        }
    },

    voteQuestion:(id, voteType) => {
        //vote types can be [down, none, up]
        var data = {
            id,
            voteType
        };
        return connectivityModule.http.post("Question/vote", data)
    },
    voteAnswer:(id, voteType) => {
        //vote types can be [down, none, up]
        var data = {
            id,
            voteType
        };
        return connectivityModule.http.post("Answer/vote", data)
    },
    voteDocument:(id, voteType) => {
        //vote types can be [down, none, up]
        var data = {
            id,
            voteType
        };
        return connectivityModule.http.post("Document/vote", data)
    },

    updateVoteCounter:(item, type)=>{
        if(type === "up"){
            if(!!item.upvoted){
                return;
            }else if(!!item.downvoted){
                item.votes = item.votes + 2
            }else{
                item.votes = item.votes + 1;
            }
            item.upvoted = true;
            item.downvoted = false;
        }else if(type === "down"){
            if(!!item.upvoted){
                item.votes = item.votes - 2
            }else if(!!item.downvoted){
                return;
            }else{
                item.votes = item.votes - 1;
            }
            item.downvoted = true;
            item.upvoted = false;
        }else{               
            if(!!item.upvoted){
                item.votes = item.votes - 1;
            }else if(!!item.downvoted){
                item.votes = item.votes + 1;
            }
            item.downvoted = false;
            item.upvoted = false; 
        }
    }
}