<template>
     <v-app>
        <component :is="layout"></component>
        <dialogInjection class="dialogInjection" />
        <componentInjection class="componentInjection" />
     </v-app>
</template>

<script>

const defaultLayout = () => import('./app.vue');
const studyRoomLayout = () => import('./studyRoomLayout.vue');


const dialogInjection = () => import('../pages/global/dialogInjection/dialogInjection.vue');
const componentInjection = () => import('../pages/global/toasterInjection/componentInjection.vue');

export default {
    components: {
        defaultLayout,
        studyRoomLayout,
        dialogInjection,
        componentInjection,
    },
    computed: {
        layout() {
            return this.$route.meta.layout || 'defaultLayout';
        }
    }
}
</script>