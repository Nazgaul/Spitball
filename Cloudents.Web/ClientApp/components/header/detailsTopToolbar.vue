<template functional>
    <div>
        <v-btn icon @click="$router.go(-1)">
            <v-icon>sbf-arrow-button</v-icon>
        </v-btn>
        <v-toolbar-title>{{props.title}}</v-toolbar-title>
    </div>
</template>