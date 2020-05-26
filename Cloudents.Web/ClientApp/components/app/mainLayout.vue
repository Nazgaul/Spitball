<template>
    <component :is="layout">
        <template v-slot:appInjections>
            <dialogInjection class="dialogInjection" />
            <componentInjection class="componentInjection" />
        </template>
    </component>
</template>

<script>

const defaultLayout = () => import('./app.vue');
const appSession = () => import('./../studyroom/appSession.vue');

//This should not be async since we can loose events if the components not loading in time
import dialogInjection from '../pages/global/dialogInjection/dialogInjection.vue';
import componentInjection from '../pages/global/toasterInjection/componentInjection.vue';

export default {
    components: {
        defaultLayout,
        appSession,
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