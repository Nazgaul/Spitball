<template>
	<div>
		
	<v-data-table :headers="fakeData"
					  :items="fakeData"
					  hide-actions
					  class="elevation-1">
	<template slot="headerCell" slot-scope="props">
	</template>
	<template v-slot:items="props">

	</template>
	</v-data-table>
	</div>
</template>

<script>
export default {
   name:'myPurchases',
   data() {
      return {
         fakeData: [
				{preview:'./Desktop.png',info:'alkfjlskjdfl',type:'Doc',status:'bla',price:36,date:'1/5/19'},
				{preview:'./Desktop.png',info:'alkfjlskjdfl',type:'Doc',status:'bla',price:36,date:'1/5/19'},
				{preview:'./Desktop.png',info:'alkfjlskjdfl',type:'Doc',status:'bla',price:36,date:'1/5/19'},
				{preview:'./Desktop.png',info:'alkfjlskjdfl',type:'Doc',status:'bla',price:36,date:'1/5/19'},
				{preview:'./Desktop.png',info:'alkfjlskjdfl',type:'Doc',status:'bla',price:36,date:'1/5/19'},
				{preview:'./Desktop.png',info:'alkfjlskjdfl',type:'Doc',status:'bla',price:36,date:'1/5/19'},
			],
			headers:['Preview','info','type','status','price','date']
      }
	},


}
</script>

<style lang="less">

</style>


