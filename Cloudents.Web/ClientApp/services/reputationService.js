import { connectivityModule } from "./connectivity.module"

export default {
    voteDocument:(id, voteType) => {
        var data = {
            id,
            voteType
        };
        return connectivityModule.http.post("Document/vote", data);
    },

    updateVoteCounter:(item, type)=>{
        if(type === "up"){
            if(!!item.upvoted){
                return;
            }else if(!!item.downvoted){
                item.votes = item.votes + 2;
            }else{
                item.votes = item.votes + 1;
            }
            item.upvoted = true;
            item.downvoted = false;
        }else if(type === "down"){
            if(!!item.upvoted){
                item.votes = item.votes - 2;
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