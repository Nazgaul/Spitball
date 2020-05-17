<template functional>
    <div class="documentPrice">
        <v-icon class="docIcon" color="#69687d" v-if="props.price" size="20">{{parent.$vuetify.icons.values.lock}}</v-icon>
        <span class="docFree" v-else v-t="'resultNote_free'"></span>
    </div>
</template>