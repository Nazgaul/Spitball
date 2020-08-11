<template>
  <v-navigation-drawer class="courseDrawer" :right="$vuetify.rtl" permanent width="338" app fixed touchless clipped>
	<div class="drawerHeader">
		<div class="ps-5 pb-4 pt-12">Edit sell page</div>
	</div>

	<div class="courseEditSections ps-5 pe-7 pt-6">
		<v-expansion-panels v-model="x" accordion flat >
			<heroEdit/>
			<classEdit v-if="isCourseSessions"/>
			<contentEdit v-if="isCourseItems"/>
			<teacherEdit/>
		</v-expansion-panels>
	</div>

	<div class="text-center pt-9">
		<v-btn class="me-3" color="#4452fc" width="132" height="40" v-t="'cancel'" rounded depressed outlined/>
		<v-btn class="white--text" color="#4452fc" width="132" height="40" v-t="'save'" depressed rounded/>
	</div>
  </v-navigation-drawer>
</template>

<script>
import heroEdit from './heroEdit.vue';
import classEdit from './classEdit.vue';
import contentEdit from './contentEdit.vue';
import teacherEdit from './teacherEdit.vue';

export default {
	components:{
		heroEdit,
		classEdit,
		contentEdit,
		teacherEdit
	},
	data() {
		return {
			x:true
		}
	},
	computed: {
		isCourseSessions(){
			return this.$store.getters.getCourseSessionsPreview?.length
		},
		isCourseItems(){
         return this.$store.getters.getCourseItems?.length
		}
	},
};
</script>

<style lang="less">
	.courseDrawer{
		.drawerHeader{
			border-bottom: solid 1px #dddddd;
			font-size: 20px;
			font-weight: 600;
			line-height: 1.15;
			color: #43425d;
		}
		.courseEditSections{

		}
	}
</style>