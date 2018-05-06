<template functional>
    <div>
        <div class="step-wrapper">
            <div class="side-wrapper data">
                <div>
                    <slot name="step-data"></slot>
                </div>
            </div>
            <div class="side-wrapper image">
                <slot name="step-image"></slot>
            </div>
        </div>
    </div>
</template>