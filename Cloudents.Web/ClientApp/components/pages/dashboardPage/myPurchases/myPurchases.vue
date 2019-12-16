<template>
	<v-layout column class="myPurchases">
		<h1>My Purchases</h1>
		<v-layout row wrap justify-end class="myPurchases_filters">
			<v-btn class="black--text" color="none" @click="reset" icon><v-icon>sbf-clearAll-icon</v-icon></v-btn>
			<v-flex sm2 xs12 :class="[{'mr-3':!isMobile}]">
				<v-select :append-icon="'sbf-arrow-down'" :items="['Video','Doc','Sessions','All']" v-model="filter" label="Type" outline/>
			</v-flex>
			<v-flex sm2 xs12 :class="[{'mr-3':!isMobile}]">
				<v-select :append-icon="'sbf-arrow-down'" :items="['Date','Price']" v-model="sort" label="Sort" outline/>
			</v-flex>
			<v-flex sm3 xs12>
				<v-text-field v-model="search" placeholder="Placeholder"/>
			</v-flex>
		</v-layout>
		<v-flex xs12 class="myPurchases_table">
			<v-data-table
				:headers="headers"
				:items="dataFilteredAndSorted"
				 disable-initial-sort
				class="elevation-1"
				:prev-icon="'sbf-arrow-left-carousel'"
				:sort-icon="'sbf-arrow-down'"
				:next-icon="'sbf-arrow-right-carousel'">
				<template v-slot:items="props">
					<td class="myPurchases_td_img">
						<img :src="require(`${props.item.preview}`)" alt="">
					</td>
					<td class="text-xs-left">
						<div>
							<span>{{ props.item.info }}</span>
							<p>Course: sdafsfsd</p>
						</div>
					</td>
					<td class="text-xs-left">{{ props.item.type }}</td>
					<td class="text-xs-left">{{ props.item.status }}</td>
					<td class="text-xs-left">{{ props.item.price }}</td>
					<td class="text-xs-left">{{ props.item.date }}</td>
					<td class="text-xs-center">
						<v-btn depressed round color="primary">{{props.item.type === 'Video'?'Watch':'Download'}}</v-btn>
					</td>
				</template>
			</v-data-table>
		</v-flex>
	</v-layout>
</template>

<script>
export default {
   name:'myPurchases',
   data() {
      return {
			search:'',
			filter:'',
			sort:'',
         fakeData: [
				{preview:'./Desktop.png',info:'maor',type:'Doc',status:'bla',price:1,date:'1/5/19'},
				{preview:'./Desktop.png',info:'ram',type:'Video',status:'bla',price:38,date:'1/5/19'},
				{preview:'./Desktop.png',info:'gab',type:'Doc',status:'bla',price:2,date:'1/5/19'},
				{preview:'./Desktop.png',info:'elad',type:'Video',status:'bla',price:39,date:'1/5/19'},
				{preview:'./Desktop.png',info:'hadar',type:'Video',status:'bla',price:3,date:'1/5/19'},
				{preview:'./Desktop.png',info:'idan2',type:'Doc',status:'bla',price:40,date:'1/5/19'},
				{preview:'./Desktop.png',info:'idan2',type:'Tutor Session',status:'bla',price:40,date:'1/5/19'},
			],
			fakeDataCopy:[
				{preview:'./Desktop.png',info:'maor',type:'Doc',status:'bla',price:1,date:'1/5/19'},
				{preview:'./Desktop.png',info:'ram',type:'Video',status:'bla',price:38,date:'1/5/19'},
				{preview:'./Desktop.png',info:'gab',type:'Doc',status:'bla',price:2,date:'1/5/19'},
				{preview:'./Desktop.png',info:'elad',type:'Video',status:'bla',price:39,date:'1/5/19'},
				{preview:'./Desktop.png',info:'hadar',type:'Video',status:'bla',price:3,date:'1/5/19'},
				{preview:'./Desktop.png',info:'idan2',type:'Doc',status:'bla',price:40,date:'1/5/19'},
			],
			 headers: [
				 {text:'Preview',align:'left',sortable: false,value:'preview'},
				 {text:'Info',align:'left',sortable: false,value:'info'},
				 {text:'Type',align:'left',sortable: true,value:'type'},
				 {text:'Status',align:'left',sortable: true,value:'status'},
				 {text:'Price',align:'left',sortable: true,value:'price'},
				 {text:'Date',align:'left',sortable: true,value:'date'},
				 {text:'Action',align:'left',sortable: false,value:'action'},
      	],
      }
	},
	methods: {
		reset(){
			this.fakeDataCopy = this.fakeData;
			this.search = '';
			this.sort = '';
			this.filter = '';
		}
	},
	computed: {
		isMobile(){
			return this.$vuetify.breakpoint.xsOnly;
		},
		dataFilteredAndSorted(){
			let data = this.fakeData;

			if(this.filter){
				if(this.filter === 'All'){
					data = this.fakeData;
				}else{
					data = data.filter(d=> d.type.includes(this.filter))
				}
			}
			if(this.sort){
				if(this.sort === 'Date'){
					data = data.sort((a,b)=> a.date - b.date)
				}else{
					data = data.sort((a,b)=> a.price - b.price)
				}
			}
			if(this.search){
				data = data.filter(d => d.info.includes(this.search))
			}
			return data;
		}
	}
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.myPurchases{
	.myPurchases_td_img{
		padding-right: 0 !important;
		width: 100px;
		img{
			height: 80px;
		}
	}
}

</style>


