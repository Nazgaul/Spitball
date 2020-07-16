<template functional>
    <div class="documentPrice">
        <v-icon class="docIcon" color="#69687d" v-if="props.price > 0 && props.isSubscribed" size="20">{{parent.$vuetify.icons.values.lock}}</v-icon>
        <span class="docFree" dir="auto" v-else>{{parent.$tc('itemCardCarousel_pts', props.price)}}</span>
    </div>
</template>