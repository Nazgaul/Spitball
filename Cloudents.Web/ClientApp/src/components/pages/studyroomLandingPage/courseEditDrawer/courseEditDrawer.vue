<template>
  <v-navigation-drawer class="courseDrawer" :right="$vuetify.rtl" permanent width="338" app fixed touchless clipped>
	<div class="drawerHeader">
		<div class="px-5 pb-4 pt-12 d-flex align-center justify-space-between">
			<div v-t="'title_edit'"/>
			<span class="white--text stepCircle">2</span>
		</div>
	</div>

	<div class="courseEditSections ps-5 pe-5 pt-6">
		<v-expansion-panels :value="0" accordion flat >
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
			.stepCircle{
				background: #4c59ff;
				width: 24px;
				height: 24px;
				border-radius: 50%;
				display: flex;
				align-items: center;
				justify-content: center;
				font-size: 13px;
				padding-bottom: 2px;
			}
		}
		.courseEditSections{

		}
	}
</style>