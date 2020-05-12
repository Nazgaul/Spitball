<template functional>
    <div class="tutorPrice d-flex" :class="[props.discount || parent.$vuetify.breakpoint.xsOnly ? 'align-baseline' : 'align-center']">
        <i18n-n :value="props.discount || props.price" tag="div"  :format="{ key: 'currency', /* currency: props.currency */ }">
            <template v-slot:integer="slotProps">
                <span class="number">{{ slotProps.integer }}</span>
            </template>
            <template v-slot:currency="slotProps">
                <span class="currency d-inline-block"> {{ slotProps.currency }}</span>
            </template>
        </i18n-n>
        /<span class="hour" v-t="'profile_points_hour'"></span>
    </div>
</template>