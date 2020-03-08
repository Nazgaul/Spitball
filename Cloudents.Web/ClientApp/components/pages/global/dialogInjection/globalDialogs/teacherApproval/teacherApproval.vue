<template>
   <v-dialog :value="true" persistent :maxWidth="'580'" :content-class="'teacherApproval'" :fullscreen="$vuetify.breakpoint.xsOnly">
       <!-- <div class="">
       </div> -->
        <div class="pa-4 text-center">
            <div class="mainTitle text-center mb-3">{{$t('teacherApproval_title')}}</div>
            <v-alert type="error" icon="mdi-firework">{{$t('teacherApproval_error')}}</v-alert>
            <div>
                <div>{{$t('teacherApproval_date')}} <b>{{$d(new Date(item.date))}}</b></div>
                <div>{{$t('teacherApproval_student')}}  <b>{{item.name}}</b></div>

                <div>{{$t('teacherApproval_session_duration')}} <b>{{item.duration | sessionDuration}}</b></div>
                <div>{{$t('teacherApproval_lesson_per_hour')}} <b>{{}}</b></div>
                <div>{{$t('teacherApproval_coupon_discount')}} <b>{{}}</b></div>
                <v-divider></v-divider>
                <div>{{$t('teacherApproval_total_session')}} <b>{{}}</b></div>
            </div>


            <div class="d-flex">
                <v-btn icon color="#5A66FF" :ripples="false" depressed><needHelpIcon/></v-btn>
                <v-spacer></v-spacer>
                <v-btn width="140" height="40" color="#4452fc" rounded outlined v-closeDialog>{{$t('teacherApproval_btn_cancel')}}</v-btn>
                <v-btn width="140" height="40" color="#4452fc" rounded class="white--text" depressed>{{$t('teacherApproval_btn_approve')}}</v-btn>
            </div>
        </div>
    </v-dialog>
</template>

<script>
// import { mapGetters } from 'vuex';
// import * as routeNames from '../../../../../../routes/routeNames.js';
import needHelpIcon from './need_help.svg'
export default {
    name: 'teacherApproval',
    components: {
        needHelpIcon
    },
    data() {
        return {
            item: null
        }
    },
    computed: {
    },
    created() {
        this.item = this.$route.params || null
    }
}
</script>

<style lang="less">

@import '../../../../../../styles/mixin.less';
.teacherApproval{
    background: #fff;

    .mainTitle {
        color: @global-purple;
        font-size: 20px;
        font-weight: 600;
    }
}
</style>