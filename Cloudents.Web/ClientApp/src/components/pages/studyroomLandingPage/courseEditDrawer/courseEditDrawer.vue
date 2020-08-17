<template>
  <v-navigation-drawer class="courseDrawer" :right="$vuetify.rtl" permanent width="338" app fixed touchless clipped>
	<div class="drawerHeader">
		<div class="ps-5 pb-4 pt-12" v-t="'title_edit'"/>
	</div>

	<div class="courseEditSections ps-5 pe-5 pt-6">
		<v-expansion-panels v-model="x" accordion flat >
			<heroEdit/>
			<classEdit v-if="isCourseSessions"/>
			<contentEdit v-if="isCourseItems"/>
			<teacherEdit/>
		</v-expansion-panels>
	</div>

	<div class="text-center px-5 pt-7">
		<v-btn @click="updateInfo" :loading="loadignBtn" class="white--text" color="#4452fc" width="132" height="40" depressed rounded>
			{{$t('next')}}
		</v-btn>
	</div>
  </v-navigation-drawer>
</template>

<script>
import heroEdit from './heroEdit.vue';
import classEdit from './classEdit.vue';
import contentEdit from './contentEdit.vue';
import teacherEdit from './teacherEdit.vue';
import { CourseUpdate } from '../../../../routes/routeNames'

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
		},
		loadignBtn(){
			return this.$store.getters.getCourseLoadingButton;
		}
	},
	methods: {
		updateInfo(){
			if(this.loadignBtn) return;

			let courseId = this.$route.params?.id;
			this.$store.dispatch('updateCourseEditedInfo',courseId).then(()=>{
				this.$router.push({
					name: CourseUpdate,
					params:{
						id:courseId
					},
					query:{
						step: 3
					}
				})
			})
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