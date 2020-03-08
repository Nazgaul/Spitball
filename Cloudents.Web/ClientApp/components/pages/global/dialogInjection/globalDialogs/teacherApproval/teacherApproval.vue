<template>
   <v-dialog :value="true" persistent :maxWidth="'840'" :content-class="'teacherApproval'" :fullscreen="$vuetify.breakpoint.xsOnly">
       <div class="pa-3">
            <div>{{$t('teacherApproval_title')}}</div>
            <v-alert type="error" icon="mdi-firework">sadssdas</v-alert>
       </div>
        <v-row class="ma-0">
            <v-col cols="6">
                <v-col>{{$t('teacherApproval_date')}} <b>{{$d(new Date(item.date))}}</b></v-col>
                <v-col>{{$t('teacherApproval_student')}}  <b>{{item.name}}</b></v-col>
                <v-col>{{$t('teacherApproval_time_record')}}  <b>{{item.duration | sessionDuration}} </b></v-col>
            </v-col>
            <v-col cols="6" class="mb-4">
                <template>
                    <v-col>{{$t('teacherApproval_coupon_used')}} <b>{{item.price}}</b></v-col>
                    <v-col>{{$t('teacherApproval_coupon_payee')}} <b>{{item.price}}</b></v-col>
                    <v-col>{{$t('teacherApproval_coupon_payout')}} <b>{{$n(item.price, 'currency')}}</b></v-col>
                </template>
            </v-col>
            
            <v-col cols="12" sm="3">
                <v-text-field :label="$t('teacherApproval_hourly_price')"></v-text-field>
            </v-col>
            <v-col cols="12" sm="3">
                <v-text-field :label="$t('teacherApproval_session_duration')" :value="item.duration | sessionDuration"></v-text-field>
            </v-col>
            <v-col cols="12" sm="3">
                <v-text-field :label="$t('teacherApproval_coupon_discount')"></v-text-field>
            </v-col>
            <v-col cols="12" sm="3">
                <v-text-field :label="$t('teacherApproval_total_session')" ></v-text-field>
            </v-col>

            <v-row class="ma-0 pa-3">
                <v-btn width="120" outlined color="#5A66FF">{{$t('teacherApproval_need_help')}}</v-btn>
                <v-spacer></v-spacer>
                <v-btn width="120" text v-closeDialog>{{$t('teacherApproval_cancel')}}</v-btn>
                <v-btn width="120" color="#5A66FF">{{$t('teacherApproval_approve')}}</v-btn>
            </v-row>
        </v-row>
    </v-dialog>
</template>

<script>
// import { mapGetters } from 'vuex';
// import * as routeNames from '../../../../../../routes/routeNames.js';
export default {
    name: 'teacherApproval',
    data() {
        return {
            item: null
        }
    },
    computed: {
    },
    created() {
        debugger
        this.item = this.$route.params || null
        
    }
}
</script>

<style lang="less">

@import '../../../../../../styles/mixin.less';
.teacherApproval{
    background: #fff;
}
</style>