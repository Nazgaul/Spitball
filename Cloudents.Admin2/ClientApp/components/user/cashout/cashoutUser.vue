<template>
    <div>
        <span v-if="showLoading">Loading List...</span>
        <span v-if="showNoResult">NO RESULTS!</span>
        <h4>Cashout List</h4>
        <v-layout>
            <v-spacer></v-spacer>
            <v-flex xs4 sm4 md4>
                <v-text-field
                        v-model="search"
                        append-icon="search"
                        label="Search"
                        single-line
                        hide-details
                ></v-text-field>
            </v-flex>
        </v-layout>

    <v-data-table
            :headers="headers"
            :items="cashOutList"
            class="elevation-1"
            :search="search"
    >
        <template slot="items" slot-scope="props">
            <td class="text-xs-right">{{ props.item.userId }}</td>
            <td class="text-xs-right">{{ props.item.cashOutPrice }}</td>
            <td class="text-xs-right">{{ props.item.userEmail }}</td>
            <td class="text-xs-right">{{new Date(props.item.cashOutTime).toUTCString()}}</td>
            <td class="text-xs-right">{{ props.item.userQueryRatio }}</td>
            <td class="text-xs-right" :class="{'suspect': props.item.isSuspect}">{{props.item.isSuspect ? "Yes" : ""}}</td>
            <td class="text-xs-right">{{ props.item.isIsrael ? "Yes" : "" }}</td>

        </template>
    </v-data-table>
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
            search: '',
            headers: [
                { text: 'User ID', value: 'userId' },
                { text: 'Cashout Price', value: 'cashoutPrice' },
                { text: 'User Email', value: 'userEmail' },
                { text: 'Date of cash out', value: 'cashDate' },
                { text: 'User Query Ratio', value: 'queryRatio' },
                { text: 'Is Suspect', value: 'isSuspect' },
                { text: 'Is From Israel', value: 'isIsarel' },
            ],
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
