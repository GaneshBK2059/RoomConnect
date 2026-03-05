import { Link } from 'react-router-dom';
import { useAuth } from '../features/auth/context/AuthContext';
import { Button } from '../components/ui/Button';
import './Home.css';

export const Home = () => {
    const { isAuthenticated, user } = useAuth();

    return (
        <div className="home-container">
            <div className="hero-section">
                <div className="hero-content">
                    <h1 className="hero-title">
                        Find Your Perfect <span className="text-gradient">Space</span>
                    </h1>
                    <p className="hero-subtitle">
                        RoomConnect makes it easy to list, discover, and rent beautiful rooms anywhere in the world.
                    </p>

                    <div className="hero-actions">
                        {!isAuthenticated ? (
                            <>
                                <Link to="/register">
                                    <Button size="lg" className="hero-btn">Get Started</Button>
                                </Link>
                                <Link to="/login">
                                    <Button variant="outline" size="lg" className="hero-btn">Sign In</Button>
                                </Link>
                            </>
                        ) : (
                            <div className="welcome-back">
                                <h2>Welcome back, {user?.fullName}!</h2>
                                <div className="dashboard-actions">
                                    <Button size="lg">Explore Rooms</Button>
                                    {user?.role === 'LANDLORD' && (
                                        <Button variant="outline" size="lg">My Listings</Button>
                                    )}
                                </div>
                            </div>
                        )}
                    </div>
                </div>

                <div className="hero-image-wrapper">
                    <div className="glass-card decorative-card card-1">
                        <div className="skeleton line-short"></div>
                        <div className="skeleton line-long"></div>
                    </div>
                    <div className="glass-card decorative-card card-2">
                        <div className="skeleton circle"></div>
                        <div className="skeleton line-long"></div>
                        <div className="skeleton line-short mt-2"></div>
                    </div>
                    <img
                        src="https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80"
                        alt="Beautiful interior room"
                        className="hero-image"
                    />
                </div>
            </div>

            <div className="features-section">
                <div className="feature">
                    <div className="feature-icon">✨</div>
                    <h3>Premium Spaces</h3>
                    <p>Discover high-quality, verified listings that match your lifestyle.</p>
                </div>
                <div className="feature">
                    <div className="feature-icon">🔒</div>
                    <h3>Secure Booking</h3>
                    <p>Book with confidence using our secure platform and payment integration.</p>
                </div>
                <div className="feature">
                    <div className="feature-icon">🤝</div>
                    <h3>Community Trust</h3>
                    <p>Read genuine reviews from real renters and trusted landlords.</p>
                </div>
            </div>

            <section className="how-it-works-section">
                <div className="section-header">
                    <h2>How It Works</h2>
                    <p>Rent your dream room in 3 simple steps</p>
                </div>
                <div className="steps-container">
                    <div className="step">
                        <div className="step-number">1</div>
                        <h3>Search</h3>
                        <p>Browse through our extensive list of verified rooms in your preferred location.</p>
                    </div>
                    <div className="step">
                        <div className="step-number">2</div>
                        <h3>Book</h3>
                        <p>Connect with landlords and secure your room securely through our platform.</p>
                    </div>
                    <div className="step">
                        <div className="step-number">3</div>
                        <h3>Move In</h3>
                        <p>Pack your bags and move into your new home with complete peace of mind.</p>
                    </div>
                </div>
            </section>

            <section className="categories-section">
                <div className="section-header">
                    <h2>Popular Categories</h2>
                    <p>Find the perfect space that fits your lifestyle</p>
                </div>
                <div className="categories-grid">
                    <div className="category-card">
                        <img src="https://images.unsplash.com/photo-1554995207-c18c203602cb?auto=format&fit=crop&q=80&w=400" alt="Private Room" />
                        <div className="category-content">
                            <h3>Private Rooms</h3>
                            <p>120+ Listings</p>
                        </div>
                    </div>
                    <div className="category-card">
                        <img src="https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&q=80&w=400" alt="Shared Space" />
                        <div className="category-content">
                            <h3>Shared Spaces</h3>
                            <p>85+ Listings</p>
                        </div>
                    </div>
                    <div className="category-card">
                        <img src="https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?auto=format&fit=crop&q=80&w=400" alt="Entire Apartment" />
                        <div className="category-content">
                            <h3>Entire Apartments</h3>
                            <p>40+ Listings</p>
                        </div>
                    </div>
                    <div className="category-card">
                        <img src="https://images.unsplash.com/photo-1536376072261-38c75010e6c9?auto=format&fit=crop&q=80&w=400" alt="Studio Apartments" />
                        <div className="category-content">
                            <h3>Studio Apartments</h3>
                            <p>65+ Listings</p>
                        </div>
                    </div>
                </div>
            </section>

            <section className="testimonials-section">
                <div className="section-header">
                    <h2>What Our Users Say</h2>
                    <p>Join thousands of happy renters and landlords</p>
                </div>
                <div className="testimonials-grid">
                    <div className="testimonial-card">
                        <div className="stars">★★★★★</div>
                        <p className="testimonial-text">"Found a beautiful room near my university within 2 days. The verification process made me feel completely safe."</p>
                        <div className="testimonial-author">
                            <div className="author-avatar">S</div>
                            <div>
                                <h4>Sarah Jenkins</h4>
                                <span>Student</span>
                            </div>
                        </div>
                    </div>
                    <div className="testimonial-card">
                        <div className="stars">★★★★★</div>
                        <p className="testimonial-text">"As a landlord, RoomConnect is a game-changer. I get quality tenants and the payment system is seamless."</p>
                        <div className="testimonial-author">
                            <div className="author-avatar">M</div>
                            <div>
                                <h4>Michael Chen</h4>
                                <span>Property Owner</span>
                            </div>
                        </div>
                    </div>
                    <div className="testimonial-card">
                        <div className="stars">★★★★★</div>
                        <p className="testimonial-text">"The photos matched reality exactly. Customer support was also super helpful when I had questions."</p>
                        <div className="testimonial-author">
                            <div className="author-avatar">E</div>
                            <div>
                                <h4>Emma Davis</h4>
                                <span>Professional</span>
                            </div>
                        </div>
                    </div>
                    <div className="testimonial-card">
                        <div className="stars">★★★★★</div>
                        <p className="testimonial-text">"I love how easy it is to communicate with potential roommates. Found a great place in no time!"</p>
                        <div className="testimonial-author">
                            <div className="author-avatar">J</div>
                            <div>
                                <h4>James Wilson</h4>
                                <span>Digital Nomad</span>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    );
};


