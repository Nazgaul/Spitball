<template>
    <v-layout class="tutor-session-row-body">
        <v-flex xs2 class="pl-3">{{session.name}}</v-flex>
        <v-flex xs2 class="pl-3">{{session.phone}}</v-flex>
        <v-flex xs2 class="pl-3">{{session.email}}</v-flex>
        <v-flex xs2 class="pl-3">{{new Date(session.created).toLocaleString('he-IL')}}</v-flex>
        <v-flex xs2 class="pl-3">{{new Date(session.ended).toLocaleString('he-IL')}}</v-flex>
        <v-flex xs2 class="pl-3">{{session.minutes}}</v-flex>
        <v-flex xs2 class="pl-3">{{session.cost}}</v-flex>
        <v-flex xs2 class="pl-3">
            <v-btn :color="isPayed ? 'success' : 'warning'" block>
                {{isPayed ? 'Paid' : 'Need to pay'}}
            </v-btn>
        </v-flex>
    </v-layout>
</template>

<script>
export default {
    props: {
        session: {
            type: Object,
        }
    },
    data() {
        return {
            loading: false,
        }
    },
    computed: {
        isPayed() {
            return this.session.isPayed;
        }
    }
}
</script>

<style lang="less">

        .tutor-session-row-body {
            width: 100%;
            padding: 4px;
            margin: 0 !important; // vuetify
            &:nth-child(odd) {
                background: #f7f7f7;
            }
            &:nth-child(even) {
                background: #fcfcfc;
            }
            div {
                overflow: hidden;
                text-overflow: ellipsis;
            }
        }
    
</style>