<template>
    <div class="pa-2 tutor-session">
        <v-layout justify-center>
            <v-flex xs12 style="background: #ffffff;">
                <v-toolbar color="indigo" class="heading-toolbar" dark>
                    <v-toolbar-title>Payment Session</v-toolbar-title>
                    <v-spacer></v-spacer>
                    <!-- <v-btn flat @click="">Clear</v-btn> -->
                    <!-- <v-combobox
                        v-model="filterWaitingFor"
                        :items="filters.waitingFor"
                        class="mr-2 top-card-select"
                        hide-details
                        menu-props="lazy"
                        box
                        outline
                        label="Waiting for"
                        validate-on-blur
                        @change="handleFilter"
                    ></v-combobox>
                    <v-combobox
                        v-model="filterStatusName"
                        class="mr-2 top-card-select"
                        hide-details
                        box
                        readonly
                        menu-props="lazy"
                        round
                        outline
                        label="Status"
                        @click.native.stop="openStatusDialog(false)"
                    ></v-combobox>
                    <v-combobox
                        v-model="filterAssignTo"
                        :items="filters.assignTo"
                        class="top-card-select"
                        hide-details
                        box
                        menu-props="lazy"
                        round
                        outline
                        label="Assigned to"
                        @change="handleFilter"
                    ></v-combobox> -->
                </v-toolbar>

                <v-card class="blue lighten-4" style="max-width:100%;">
                    <v-container fluid grid-list-lg>
                        <v-layout row wrap>

                            <v-expansion-panel class="elevation-0">
                                <v-expansion-panel-content
                                    hide-actions
                                    xs12
                                    v-for="(tutor, index) in tutors"
                                    :key="index"
                                    class="mb-3 elevation-4 card-conversation">
                                    <div slot="header" class="card-conversation-wrap" @click="getTutorSession(tutor.tutorId)">
                                        <v-layout class="card-converstaion-header">
                                            <v-flex xs2 class="pl-3">Name</v-flex>
                                            <v-flex xs2 class="pl-3">Phone</v-flex>
                                            <v-flex xs2 class="pl-3">Email</v-flex>
                                            <v-flex xs2 class="pl-3">Total Hours</v-flex>
                                            <v-flex xs2 class="pl-3">Total Students</v-flex>
                                            <v-flex xs2 class="pl-3">Price</v-flex>
                                            <v-flex xs2 class="pl-3">Payme</v-flex>
                                            <v-flex xs2 class="pl-3">State</v-flex>
                                        </v-layout>

                                        <v-layout class="card-converstaion-content">
                                            <v-flex xs2 class="card-converstaion-content-col-1 pl-3">
                                                <v-layout row wrap>
                                                        {{tutor.name}}
                                                </v-layout>
                                            </v-flex>
                                            <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-2 pl-3">
                                                <v-layout row wrap>
                                                    {{tutor.phoneNumber}} 
                                                </v-layout>
                                            </v-flex>
                                            <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-3 pl-3">
                                                <v-layout row wrap>
                                                    {{tutor.email}}
                                                </v-layout>
                                            </v-flex>
                                            <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-4 pl-3">
                                                <v-layout row wrap>
                                                    {{tutor.totalHours}}
                                                </v-layout>
                                            </v-flex>
                                            <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-5 pl-3">
                                                <v-layout>
                                                        {{tutor.totalStudents}}
                                                </v-layout>
                                            </v-flex>
                                            <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-6 pl-3">
                                                <v-layout>
                                                        {{tutor.price}}
                                                </v-layout>
                                            </v-flex>
                                            <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-7 pl-3">
                                                <div>
                                                    {{tutor.payme}}
                                                </div>
                                            </v-flex>
                                            <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-8 pl-3">
                                                <v-btn class="" :color="!tutor.needToPay ? 'success' : 'warning'" block @click.stop.prevent="">
                                                    {{!tutor.needToPay ? 'Paid' : 'Need to pay'}}
                                                </v-btn>
                                            </v-flex>
                                            <!-- TODO: Add Payment -->
                                            <!-- <v-divider vertical class="my-2"></v-divider>
                                            <v-flex xs2 class="card-converstaion-content-col-9 pl-3">
                                                <v-btn class="" color="primary" @click.stop.prevent="openDialog(tutor)">
                                                    Add
                                                </v-btn>
                                            </v-flex> -->
                                        </v-layout>
                                    </div>
                                    <div class="pa-2 tutor-session-bill">
                                        <v-layout class="tutor-session-row-header" v-if="tutorSession.length > 0 && loading">
                                            <v-flex xs2 class="pl-3 font-weight-bold">Name</v-flex>
                                            <v-flex xs2 class="pl-3 font-weight-bold">Phone</v-flex>
                                            <v-flex xs2 class="pl-3 font-weight-bold">Email</v-flex>
                                            <v-flex xs2 class="pl-3 font-weight-bold">Start</v-flex>
                                            <v-flex xs2 class="pl-3 font-weight-bold">End</v-flex>
                                            <v-flex xs2 class="pl-3 font-weight-bold">Min</v-flex>
                                            <v-flex xs2 class="pl-3 font-weight-bold">Cost (Min*Price)</v-flex>
                                            <v-flex xs2 class="pl-3 font-weight-bold">Status</v-flex>
                                        </v-layout>
                                        <v-layout v-if="tutorSession.length === 0 && !loading" class="tutor-session-noData mt-2">
                                            No Data Availlable
                                        </v-layout>
                                        <v-progress-linear slot="progress" color="blue" v-show="loading && !tutorSession.length" indeterminate></v-progress-linear>
                                        <tutor-bills-session v-for="(session, j) in tutorSession" :key="j" :session="session" />
                                    </div>
                                </v-expansion-panel-content>
                            </v-expansion-panel>                
                        </v-layout>
                    </v-container>
                </v-card>
            </v-flex>
        </v-layout>
        <!-- TODO: Add Payment -->
        <!-- <v-dialog v-model="dialog" width="500" v-if="dialog">
            <payment-dialog :tutor="currentTutor" />
        </v-dialog> -->
    </div>
</template>

<script>
import paymentSessionService from '../../../services/paymentSessionService';
import tutorBillsSession from './tutorBillsSession.vue';
import paymentDialog from './paymentDialog.vue';

export default {
    components: {
        tutorBillsSession,
        paymentDialog
    },
    data() {
        return {
            tutors: [],
            tutorSession: [],
            currentId: null,
            currentTutor: null,
            loading: false,
            dialog: false
        }
    },
    methods: {
        getTutorSession(tutorId) {
            if(this.currentId !== tutorId) {
                this.loading = true;
                this.tutorSession = [];
                paymentSessionService.getTutorPaymentBills(tutorId).then(res => {
                    this.currentId = tutorId;
                    this.tutorSession = res;
                }).finally(()=> {
                    this.loading = false
                })
            }
        },
        openDialog(tutor) {
            this.dialog = true;
            this.currentTutor = tutor
        }
    },
    created() {
        paymentSessionService.getTutorPaymentSession().then((res) => {
            this.tutors = res
        })
    }
}
</script>

<style lang="less">

    .tutor-session {
        width: 100%;
        .card-converstaion-header {
            div {
                max-width: 170px;
            }
        }
        .card-converstaion-content {
            align-items: center;
            div {
                overflow: hidden;
                text-overflow: ellipsis;
            }
            .card-converstaion-content-col-9 {
                text-align: center;
            }
        }
        .tutor-session-bill {
            margin-top: 10px;
            height: 256px;
            overflow: auto;
            .tutor-session-row-header {
                border-bottom: 1px solid #eee;
                margin: 0 !important;
            }
            .tutor-session-noData {
                display: flex;
                justify-content: center;
                font-size: 16px;
            }
        }
    }

</style>
