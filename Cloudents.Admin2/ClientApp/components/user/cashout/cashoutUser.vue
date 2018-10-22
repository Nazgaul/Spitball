<template>
    <div>
        <h1>Cashout List</h1>
        <div class="cashout-table-container" >
            <span v-if="showLoading">Loading List...</span>
            <span v-if="showNoResult">NO RESULTS!</span>
            <table v-if="cashOutList.length > 0" class="cashout-table">
                <th>User Id</th>
                <th>Cashout Price</th>
                <th>User Email</th>
                <th>Cashout Time</th>
                <th>Fraud Score</th>
                <th>User Query Ratio</th>
                <th>Is Suspect</th>
                <tbody v-for="(user,index) in cashOutList" :key="index">
                    <td>{{user.userId}}</td>
                    <td>{{user.cashOutPrice}}</td>
                    <td>{{user.userEmail}}</td>
                    <td>{{new Date(user.cashOutTime).toUTCString()}}</td>
                    <td>{{user.fraudScore}}</td>
                    <td>{{user.userQueryRatio}}</td>
                    <td :class="{'suspect': user.isSuspect}">{{user.isSuspect}}</td>
                </tbody>
            </table>
        </div>
    </div>
</template>
<script>
import {getCashoutList} from './cashoutUserService'
export default {
    data(){
        return{
            cashOutList:[],
            showLoading:true,
            showNoResult:false,
        }
    },
    created(){
        getCashoutList().then((list)=>{
            if(list.length === 0){
                this.showNoResult=true;
            }else{
                this.cashOutList = list;
            }
                this.showLoading = false;
        },(err)=>{
            console.log(err)
        })
    }
}
</script>

<style lang="scss" scoped>
.cashout-table-container{
    .cashout-table{
        margin: 0 auto;
        text-align: center;
        vertical-align: middle;
        width:90%;
        td{
            border: 2px solid #b6b6b6;
            border-radius: 18px;
            &.suspect{
                background-color:#ff9b9b;
                font-weight: 600;
            }
        }
        th{
            border: 2px solid #b6b6b6;
            border-radius: 18px;
            background-color: #b6b6b6;
        }
    }
    
}
</style>
