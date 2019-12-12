<template>
      <div class="mySales">
            <v-data-table
               border="1"
               :headers="headers"
               :items="salesItems"
               disable-initial-sort
               rows-per-page-items
               class="elevation-1"
               :prev-icon="'sbf-arrow-left-carousel'"
               :sort-icon="'sbf-arrow-down'"
               :next-icon="'sbf-arrow-right-carousel'">
               <template v-slot:items="props">
                  <td class="mySales_td_img">
                     <img :src="props.item.preview" :alt="props.item.info" v-if="props.item.preview">
                     <v-icon v-else>sbf-user</v-icon>
                  </td>
                  <td class="text-xs-left">{{ props.item.info }}</td>
                  <td class="text-xs-left">{{ props.item.type }}</td>
                  <!-- <td class="text-xs-left">{{ props.item.likes }}</td> -->
                  <!-- <td class="text-xs-left">{{ props.item.views }}</td> -->
                  <!-- <td class="text-xs-left">{{ props.item.downloads }}</td> -->
                  <!-- <td class="text-xs-left">{{ props.item.purchased }}</td> -->
                  <!-- <td class="text-xs-left">{{ props.item.price }}</td> -->
                  <td class="text-xs-left">{{ props.item.status }}</td>
                  <td class="text-xs-left">{{ props.item.date | dateFromISO }}</td>
                  <!-- <td class="text-xs-left"></td> -->
               </template>
            </v-data-table>
      </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

export default {
   name:'mySales',
   data() {
      return {
         headers:[
            {text:'Preview',align:'left',sortable: false,value:'preview'},
            {text:'Info',align:'left',sortable: false,value:'info'},
            {text:'Type',align:'left',sortable: true,value:'type'},
            // {text:'Likes',align:'left',sortable: true,value:'likes'},
            // {text:'Views',align:'left',sortable: true,value:'views'},
            // {text:'Downloads',align:'left',sortable: true,value:'downloads'},
            // {text:'Purchased',align:'left',sortable: true,value:'purchased'},
            // {text:'Price',align:'left',sortable: true,value:'price'},
            {text:'Status',align:'left',sortable: true,value:'status'},
            {text:'Date',align:'left',sortable: true,value:'date'},
            // {text:'Action',align:'left',sortable: false,value:'action'},
         ],
      }
   },
   computed: {
      ...mapGetters(['getSalesItems']),
      salesItems(){
         return this.getSalesItems;
      },
   },
   methods: {
      ...mapActions(['updateSalesItems'])
   },
   created() {
      this.updateSalesItems()
   },
}
</script>

<style lang="less">
.mySales{
   .v-table {
      thead {
         th{
            // border: 1px solid black;
            // border-bottom: 0;
         }
      }
      tbody {
         td{
            padding: 8px 24px !important; //vuetify
            // border: 1px solid black;
         }
      }
   }

   .mySales_td_img{
		padding-right: 0 !important;
		width: 100px;
		img{
			height: 80px;
      }
      i {
         // Temporary
         font-size: 70px;
      }
	}
}
</style>