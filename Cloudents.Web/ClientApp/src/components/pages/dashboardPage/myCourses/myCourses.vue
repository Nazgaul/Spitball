<template>
   <div class="myCourses">
      <v-data-table 
            @click:row="handleRowClick"
            :headers="headers"
            :items="coursesItems"
            :items-per-page="5"
            :mobile-breakpoint="0"
            :item-key="'itemId'"
            sort-by
            class="myCourses_table"
            :footer-props="{
               showFirstLastPage: false,
               firstIcon: '',
               lastIcon: '',
               itemsPerPageOptions: [5]
            }">

            <template v-slot:top>
               <div class="tableTop">
                  <div class="myStudyRooms_title pb-3 pb-sm-0" v-t="'my_courses'"></div>
                  <div class="text-end">
                     <v-btn
                        :to="{name: createCourseRoute}"
                        class="white--text"
                        depressed
                        width="180"
                        rounded
                        :block="$vuetify.breakpoint.xsOnly"
                        color="#5360FC"
                     >
                        <v-icon size="22" left>sbf-plus-circle</v-icon>
                        <span v-t="'dashboardPage_my_content_upload'"></span>
                     </v-btn>
                  </div>
               </div>
            </template>

            <template v-slot:item.preview="{item}">
                  <div class="tablePreview">
                     <img v-show="isLoaded" @load="loaded" :src="$proccessImageUrl(item.image, 127, 80)" class="tablePreview_img" width="127" height="80" />
                     <v-skeleton-loader v-if="!isLoaded" height="80" width="127" type="image"></v-skeleton-loader>
                  </div>
            </template>

            <template v-slot:item.name="{item}">
               <div style="max-width: 200px">
                  {{item.name}}
               </div>
            </template>

            <template v-slot:item.startOn="{item}">
               <div>{{$d(item.startOn)}}</div>
            </template>

            <template v-slot:item.users="{item}">
               <div class="d-flex">
                  <v-icon size="14">sbf-groupPersons</v-icon>
                  <div class="ms-2">{{item.users}}</div>
               </div>
            </template>

            <template v-slot:item.price="{item}">
               <div>{{$price(item.price.amount, item.price.currency, true)}}</div>
            </template>

            <template v-slot:item.isPublish="{item}">
               <div class="d-flex">
                  <div>{{item.isPublish ? $t('visible') : $t('notVisible')}}</div>
               </div>
            </template>

            <template v-slot:item.action="{item}">
               <v-menu offset-y>
                  <template v-slot:activator="{ on }">
                     <v-btn icon>
                        <v-icon size="18" v-on="on">sbf-3-dot</v-icon>
                     </v-btn>
                  </template>
                  <v-list>
                  <v-list-item
                     :to="{name: uppdateCourseRoute, params: { id: item.id }}"
                  >
                     <v-list-item-title v-t="'go_edit'"></v-list-item-title>
                  </v-list-item>
                  </v-list>
               </v-menu>
            </template>
            <slot slot="no-data" name="tableEmptyState"/>
      </v-data-table>
   </div>
</template>

<script>
import { mapGetters } from 'vuex';
import { CourseCreate, CourseUpdate, StudyRoomLanding } from '../../../../routes/routeNames'

export default {
   name:'myCourses',
   data() {
      return {

         isLoaded: false,
         uppdateCourseRoute: CourseUpdate,
         createCourseRoute: CourseCreate,
         headers: [
            {text: '', align:'', sortable: false, value:'preview'},
            {text: this.$t('dashboardPage_course_name'), align:'', sortable: false, value:'name'},
            {text: this.$t('dashboardPage_startOn'), align:'', sortable: false, value:'startOn'},
            {text: this.$t('dashboardPage_lecture'), align:'', sortable: false, value:'lessons'},
            {text: this.$t('dashboardPage_resource'), align:'', sortable: false, value:'documents'},
            {text: this.$t('dashboardPage_enroll'), align:'', sortable: false, value:'users'},
            {text:this.$t('dashboardPage_price'), align:'', sortable: true, value:'price'},
            {text: this.$t('dashboardPage_status'), align:'', sortable: true, value:'isPublish'},
            {text: '', align:'', sortable: false, value:'action'},
         ]
      }
   },
   computed: {
      ...mapGetters(['getCoursesItems']),
      coursesItems(){
         // avoiding duplicate key becuase we have id that are the same,
         // vuetify default key is "id", making new key "itemId" for unique index table items
         return this.getCoursesItems && this.getCoursesItems.map((item, index) => {
            return {
               itemId: index,
               ...item
            }
         })
      }
   },
   methods: {
      loaded() {
         this.isLoaded = true;
      },
      handleRowClick(item) {
         if(!item.isPublish) {
            return
         }
         this.$router.push({name: StudyRoomLanding, params: { id: item.id }})
      },
   },
   beforeDestroy() {
      this.$store.commit('resetCourseItems')
   },
   created() {
      this.$store.dispatch('updateCoursesItems')
   },
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@import "../../../../styles/colors.less";

.myCourses{
   max-width: 1366px;
   .myCourses_table {
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
      thead {
         tr {
            height: auto;
            th{
               color: @global-purple !important;
               font-size: 14px;
               padding-top: 14px;
               padding-bottom: 14px;
               font-weight: normal;
            }
         }
         color: @global-purple !important;
      }
      .tablePreview{
         line-height: 0;
         padding-right: 0 !important;
         width: 104px;
         position: relative;
         .tablePreview_img{
            margin: 10px 0;
            border: 1px solid #d8d8d8;
         }
      }
      .tableTop {
         padding: 20px;
         color: @global-purple !important;
         .myStudyRooms_title {
            font-size: 22px;
            font-weight: 600;
            // line-height: 1.3px;
            @media (max-width: @screen-xs) {
            line-height: initial;
            }
            background: #fff;
         }
         .link {
            color: inherit;
            font-weight: 600;
            &.btnTestStudyRoom {
            border: 1px solid #5360FC;
            color: #5360FC;
            }
         }
         a {
            text-transform: initial;
         }
      }

      tbody tr {
         td {
            border-bottom: none !important;
            cursor:pointer;
         }
         td:nth-child(2) {
            padding-left: 0;
            cursor:pointer;
            
         }
      }
      tr:nth-child(even) {
         background-color: #f5f5f5;
         cursor:pointer;
      }
      .tableInfo{
         width: 400px;
         max-width: 400px;
         min-width: 300px;
         .tableInfo_router{
            color: @global-purple !important;
            line-height: 1.6;
         }
      }

      .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         color: @global-purple !important;
         height: inherit;
         font-size: 14px;
      }
      .v-data-footer {
         padding: 6px 0;
         .v-data-footer__pagination {
            font-size: 14px;
            color: @global-purple;
         }
      }
   }
}
</style>