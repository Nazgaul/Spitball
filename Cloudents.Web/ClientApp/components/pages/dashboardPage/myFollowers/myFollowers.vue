<template>
   <div class="myFollowers">
      <h1>myFollowers</h1>
      <v-layout wrap align-baseline>
         <v-btn depressed color="primary">Send Email</v-btn>
         <v-btn depressed color="primary">Chat</v-btn>
         <v-btn depressed color="primary">Make a Billing</v-btn>
         <v-btn depressed color="primary">Make an appointment</v-btn>
         <input v-model="search" class="myFollowers_search" type="text" placeholder="Search">
      </v-layout>
      <v-data-table
				:headers="headers"
            hide-actions
				:items="dataFilteredAndSorted"
				class="elevation-1"
				:prev-icon="'sbf-arrow-left-carousel'"
				:sort-icon="'sbf-arrow-down'"
				:next-icon="'sbf-arrow-right-carousel'">
				<template v-slot:items="props">
               <td class="myFollowers_td_select">
                  <v-checkbox :ripple="false" 
                              v-model="selectedUser" 
                              hide-details :value="props.item.name"
                              off-icon="sbf-check-box-un" 
                              on-icon="sbf-check-box-done"/>
               </td>
               <td>{{props.item.name}}</td>
               <td>{{props.item.joined}}</td>
               <td>{{props.item.type}}</td>
               <td>some btns</td>
				</template>
			</v-data-table>
   </div>
</template>

<script>
export default {
   name:'myFollowers',
   data() {
      return {
         selectedUser:false,
         search:'',
         headers: [
            {text:'Select',align:'left',sortable: false,value:'select'},
            {text:'Name',align:'left',sortable: false,value:'name'},
            {text:'Joined',align:'left',sortable: true,value:'joined'},
            {text:'Type',align:'left',sortable: false,value:'type'},
            {text:'Action',align:'left',sortable: false,value:'action'},
         ],
         fakeData: [
            {name:'Haim Moshe',joined:'11/19',type:'Follower'}, 
            {name:'Eidan Apelbaum',joined:'11/19',type:'Purchaser'}, 
            {name:'jaron',joined:'11/19',type:'Student'}, 
            {name:'Moshe',joined:'11/19',type:'Follower'}, 
			],
      }
   },
   computed: {
      dataFilteredAndSorted(){
         if(this.search.trim()){
            return this.fakeData.filter(d=> d.name.toLowerCase().includes(this.search.trim().toLowerCase()));
         } else{
            return this.fakeData;
         }
      }
   },
}
</script>

<style lang="less">
.myFollowers{
   .myFollowers_search{
      border: 1px solid black;
      border-radius: 4px;
      height: 36px;
      padding-left: 10px;
   }
   .myFollowers_td_select{
      padding: 0;
      // .v-input__slot{
      //    margin: 0;
      // }
   }

}
</style>