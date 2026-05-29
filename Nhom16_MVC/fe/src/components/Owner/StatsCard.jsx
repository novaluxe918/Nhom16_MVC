const StatsCard = ({ title, value, description, color }) => {
    return (
        <div className="bg-white rounded-2xl p-8 shadow">
            <p className="text-gray-500 font-semibold">
                {title}
            </p>

            <h2 className={`text-5xl font-black mt-4 ${color}`}>
                {value}
            </h2>

            <p className="mt-2 text-gray-500">
                {description}
            </p>
        </div>
    );
};

export default StatsCard;